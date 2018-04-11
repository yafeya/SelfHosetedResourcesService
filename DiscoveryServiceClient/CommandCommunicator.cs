using Agilent.TMFramework.InstrumentIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Interop
{
    class CommandCommunicator
    {
        private const string DefaultEolSequence = "\n";
        private const int DefaultReadBufferSize = 1024;
        private const int DefaultConnectTimeout = 2000;
        private const int DefaultCommunicationTimeout = 5000;
        private int mVi = -1;
        private int mViDefRm = -1;

        public ScpiResult SendReadCommand(string address, string command)
        {
            var connectResult = Connect(address);
            if (!connectResult.Succeed) return connectResult;

            var result = new ScpiResult();
            var writeResult = DoWriteToInstrument(command);
            if (writeResult.Succeed)
            {
                result = ReadString();
            }
            else
            {
                result.Message = writeResult.Message;
            }

            Disconnect();
            return result;
        }

        private ScpiResult Connect(string address)
        {
            int viDefRm = -1;
            var openDefaultRm = viOpenDefaultRm(out viDefRm);
            if (!openDefaultRm.Succeed) return openDefaultRm;

            mViDefRm = viDefRm;
            int vi = -1;
            var viOpenResult = viOpen(address, viDefRm, out vi);
            if (!viOpenResult.Succeed) return viOpenResult;

            mVi = vi;
            var terminationResult = enableTermination(vi);
            if (!terminationResult.Succeed) return terminationResult;

            var setTimeoutResult = SetTimeout(vi, DefaultCommunicationTimeout);
            if (!setTimeoutResult.Succeed) return setTimeoutResult;

            var result = new ScpiResult();
            result.Succeed = true;
            return result;
        }

        private ScpiResult DoWriteToInstrument(string command)
        {
            var result = new ScpiResult();
            if (mVi == VisaInteropConstant.VI_NULL) return result;

            try
            {
                int numRead = 0;
                byte[] buf = Encoding.ASCII.GetBytes($"{command}{DefaultEolSequence}");
                int status = VisaInterop.viWrite(mVi, buf, buf.Length, ref numRead);
                if (VisaInteropUtil.Failed(status))
                {
                    result.Message = VisaInteropUtil.GetStatusDescription(mVi, status);
                }
                else
                {
                    result.Succeed = true;
                }
            }
            finally
            {

            }
            return result;
        }

        private ScpiResult ReadString()
        {
            var result = new ScpiResult();
            var response = new List<string>();
            if (mVi == VisaInteropConstant.VI_NULL) return result;

            int status;
            // StringBuilder strBld = new StringBuilder(mReadBufferSize);            
            try
            {
                byte[] buf = new byte[DefaultReadBufferSize];
                int numRead;
                int binBlockBytesToRead = 0;
                bool isBinBlock = false;
                bool checkedForBinBlock = false;

                while (true)
                {
                    // Read up to a full buffer's worth of data
                    numRead = 0;
                    status = VisaInterop.viRead(mVi, buf, buf.Length, ref numRead);

                    if (VisaInteropUtil.Failed(status))
                    {
                        result.Message = VisaInteropUtil.GetStatusDescription(mVi, status);
                    }

                    // shiqiang: copy data to new allocaed byte array and add byte array to response
                    byte[] readBuf = new byte[numRead];
                    for (int i = 0; i < numRead; i++)
                    {
                        readBuf[i] = buf[i];
                    }
                    var read = Encoding.ASCII.GetString(readBuf);
                    response.Add($"{read}{Environment.NewLine}");

                    //Append data in buffer the rest of the data that's been read
                    //string str = Encoding.ASCII.GetString(buf, 0, numRead);
                    //strBld.Append(str);

                    // On first block of data read, determine if we are dealing with an IEEE 488.2
                    // definite length block.
                    if (!checkedForBinBlock)
                    {
                        isBinBlock = IsValid4882BlockHeader(readBuf, out binBlockBytesToRead);
                        checkedForBinBlock = true;
                    }

                    // Handle 488.2 definite length binblocks by attempting to read all the data
                    // regardless of term chars.
                    if (isBinBlock)
                    {
                        binBlockBytesToRead -= numRead;
                    }

                    // Determine whether or not there is more data to read
                    if (status == VisaInteropConstant.VI_SUCCESS)
                    {
                        result.Succeed = true;
                        // VI_SUCCESS indicates END was received - no more data.
                        break;
                    }
                    else if (status == VisaInteropStatus.VI_SUCCESS_MAX_CNT)
                    {
                        // VI_SUCCESS_MAX_CNT indicates we filled the buffer and
                        // there is more data (otherwise we would have gotten either
                        // VI_SUCCESS or VI_SUCCESS_TERM_CHAR).
                        continue;
                    }
                    else if (status == VisaInteropStatus.VI_SUCCESS_TERM_CHAR)
                    {
                        if (isBinBlock && (binBlockBytesToRead > 0))
                        {
                            // If we are processing a binary block, read until we've read
                            // all the bytes indicated in the header. Since the data might
                            // contain term chars, don't end prematurely on those.
                            continue;
                        }
                        else
                        {
                            result.Succeed = true;
                            // For serial and socket, VI_SUCCESS_TERM_CHAR indicates no more data.
                            break;
                        }
                    }
                }
            }
            // shiqiag: catch possible exceptions include OutOfMemoryException
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            if (result.Succeed)
            {
                var builder = new StringBuilder();
                foreach(var item in response)
                {
                    builder.Append(item);
                }
                result.Message = builder.ToString();
            }

            return result;
        }


        private void Disconnect()
        {
            if (mVi != VisaInteropConstant.VI_NULL)
            {
                VisaInterop.viClose(mVi);
                mVi = VisaInteropConstant.VI_NULL;
            }

            if (mViDefRm != VisaInteropConstant.VI_NULL)
            {
                VisaInterop.viClose(mViDefRm);
                mViDefRm = VisaInteropConstant.VI_NULL;
            }
        }

        private static ScpiResult viOpenDefaultRm(out int viDefRm)
        {
            var result = new ScpiResult();
            viDefRm = -1;
            int status = VisaInterop.viOpenDefaultRM(ref viDefRm);
            if (VisaInteropUtil.Failed(status))
            {
                result.Succeed = false;
                result.Message = VisaInteropUtil.GetStatusDescription(viDefRm, status);
            }
            else
            {
                result.Succeed = true;
            }

            return result;
        }
        private static ScpiResult viOpen(string address, int viDefRm, out int vi)
        {
            vi = -1;
            var result = new ScpiResult();
            var status = VisaInterop.viOpen(viDefRm, address, VisaInteropConstant.VI_NO_LOCK, DefaultConnectTimeout, ref vi);
            if (VisaInteropUtil.Failed(status))
            {
                result.Succeed = false;
                result.Message = VisaInteropUtil.GetStatusDescription(viDefRm, status);
            }
            else
            {
                result.Succeed = true;
            }
            return result;
        }
        private static ScpiResult enableTermination(int vi)
        {
            var result = new ScpiResult();
            // Enable termination character if required for this session type (serial and socket).
            if (IsTermCharTerminationRequired(vi))
            {
                var status = VisaInterop.viSetAttribute(vi, VisaInteropAttribute.VI_ATTR_TERMCHAR_EN, 1);
                if (VisaInteropUtil.Failed(status))
                {
                    result.Succeed = false;
                    result.Message = VisaInteropUtil.GetStatusDescription(vi, status);
                }
                else
                {
                    result.Succeed = true;
                }
            }
            return result;
        }
        private ScpiResult SetTimeout(int vi, int timeout)
        {
            var result = new ScpiResult();
            int attr = VisaInteropAttribute.VI_ATTR_TMO_VALUE;
            int status = VisaInterop.viSetAttribute(vi, attr, timeout);
            if (VisaInteropUtil.Failed(status))
            {
                result.Succeed = false;
                result.Message = VisaInteropUtil.GetStatusDescription(vi, status);
            }
            else
            {
                result.Succeed = true;
            }
            return result;
        }

        private static bool IsTermCharTerminationRequired(int vi)
        {
            bool result = false;

            StringBuilder strBld = new StringBuilder(256);
            int status = VisaInterop.viGetAttribute(vi, VisaInteropAttribute.VI_ATTR_RSRC_CLASS, strBld);
            if (VisaInteropUtil.Succeeded(status))
            {
                string resourceClass = strBld.ToString();
                if (String.Compare(resourceClass, "SOCKET", true) == 0)
                {
                    // Check for SOCKET resource class, session needs termchar termination
                    result = true;
                }
                else if (String.Compare(resourceClass, "INSTR", true) == 0)
                {
                    // For INSTR, check if we are serial or remote serial and if so, then session
                    // needs termchar termination.
                    short intfType = 0;
                    status = VisaInterop.viGetAttribute(vi, VisaInteropAttribute.VI_ATTR_INTF_TYPE, ref intfType);
                    if (VisaInteropUtil.Succeeded(status))
                    {
                        if (intfType == VisaInteropConstant.VI_INTF_TCPIP)
                        {
                            short val = 0;
                            int attr = VisaInteropAgilentExtensions.VI_AGATTR_REMOTE_INTF_TYPE;
                            status = VisaInterop.viGetAttribute(vi, attr, ref val);
                            if (VisaInteropUtil.Succeeded(status))
                            {
                                intfType = val;
                                result = true;
                            }
                        }

                        if (intfType == VisaInteropConstant.VI_INTF_ASRL)
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        private static bool IsValid4882BlockHeader(byte[] data, out int binBlockLength)
        {
            bool result = false;
            binBlockLength = -1;
            if (data != null)
            {
                if ((data.Length >= 3) && (data[0] == '#') && (data[1] >= '0') && (data[1] <= '9'))
                {
                    byte numberOfDataLengthDigits = (byte)(data[1] - '0');
                    if (data.Length >= numberOfDataLengthDigits + 2)
                    {
                        //string lengthStr = data.Substring(2, numberOfDataLengthDigits);
                        try
                        {
                            //int dataLength = Int32.Parse(lengthStr);
                            //shiqiang: calculate the length of data
                            int dataLength = 0;
                            int startPos = 2;
                            for (int pos = startPos; pos < startPos + numberOfDataLengthDigits; pos++)
                            {
                                dataLength = dataLength * 10 + (data[pos] - '0');
                            }

                            result = true;
                            binBlockLength = 2 + numberOfDataLengthDigits + dataLength;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                }
            }

            return result;
        }
    }
}
