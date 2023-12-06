using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprise;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

namespace UMSV
{
    public partial class SipService
    {
        // Hardcoded parameters: Maximum Concurrent FAX:10 , TimeOUT:Pages*60 sec, Maximum Pages:4  

        //Return 1:success, -1:all channels are inuse, -2:tiff_error, -3:TimeOut
        //Parameters: is_sender (1->act as a fax sender, 0-> act as a fax receiver)
        [DllImport("dsp.dll")]
        public static extern int Initialize_FAX(Int16[] pcm_input, int input_len, String file_to_send, int is_sender, String tel);

        //Return 1:success, -1:Buffer is empty. Must send again after a while
        [DllImport("dsp.dll")]
        public static extern int FAX_transceiver(Int16[] pcm_output, Int16[] pcm_input, int len, String tel, Byte[] state_log);

        [DllImport("dsp.dll")]
        public static extern int g711_decode(int type, Int16[] pcm_output, byte[] g711_input, int g711_len);

        [DllImport("dsp.dll")]
        public static extern int g711_encode(int type, byte[] g711_output, Int16[] pcm_input, int pcm_len);

        // mode: 0:no log, 1:log t30 signalling
        [DllImport("dsp.dll")]
        public static extern void Enable_logging(int mode);



        //public MemoryStream PassThroughFAXInitialStream = new MemoryStream();

        int type = 0;  // ALAW:0 , ULAW:1
        public int Initial_Fax_Chunk_size = 6400; //Set between 4000 and 8000-chunk_size
        //public string file_name = "D:/Folder/itutests.tif"; //Copy tiff file with similar name in dll directory
        //public string file_name1 = "D:/Folder/itutests1.tif"; //Copy tiff file with similar name in dll directory

        const byte T30_INDICATOR = 0;
        const byte T30_DATA = 1;

        public byte[] V_21_Preamble
        {
            get
            {
                byte[] result = new byte[0];
                for (int i = 0; i < 45; i++)
                {
                    result = Combine(result, WAVE_1850Hz_1PACK_SAMPLES, WAVE_1650Hz_6PACKS_SAMPLES, WAVE_1850Hz_1PACK_SAMPLES);
                }
                return result;
            }
        }

        public byte[] DCS
        {
            get
            {
                byte[] result = new byte[0];
                byte[] DCSByteArray = { 126, 255, 200, 193, 0, 0, 0, 207, 201, 126 };

                result = Combine(result, WAVE_1850Hz_1PACK_SAMPLES, WAVE_1650Hz_6PACKS_SAMPLES, WAVE_1850Hz_1PACK_SAMPLES);//01111110
                result = Combine(result, WAVE_1650Hz_8PACKS_SAMPLES);//11111111
                result = Combine(result, WAVE_1650Hz_2PACKS_SAMPLES, WAVE_1850Hz_2PACKS_SAMPLES, WAVE_1650Hz_1PACK_SAMPLES, WAVE_1850Hz_3PACKS_SAMPLES);//11001000
                result = Combine(result, WAVE_1650Hz_2PACKS_SAMPLES, WAVE_1850Hz_5PACKS_SAMPLES, WAVE_1650Hz_1PACK_SAMPLES);//11000001
                result = Combine(result, WAVE_1850Hz_8PACKS_SAMPLES);//00000000
                result = Combine(result, WAVE_1850Hz_8PACKS_SAMPLES);//00000000
                result = Combine(result, WAVE_1850Hz_8PACKS_SAMPLES);//00000000
                result = Combine(result, WAVE_1650Hz_2PACKS_SAMPLES, WAVE_1850Hz_2PACKS_SAMPLES, WAVE_1650Hz_4PACKS_SAMPLES);//11001111
                result = Combine(result, WAVE_1650Hz_2PACKS_SAMPLES, WAVE_1850Hz_2PACKS_SAMPLES, WAVE_1650Hz_1PACK_SAMPLES, WAVE_1850Hz_2PACKS_SAMPLES, WAVE_1650Hz_1PACK_SAMPLES);//11001001
                result = Combine(result, WAVE_1850Hz_1PACK_SAMPLES, WAVE_1650Hz_6PACKS_SAMPLES, WAVE_1850Hz_1PACK_SAMPLES);//01111110

                return result;
            }
        }

        public byte[] DCSWithPreamble
        {
            get
            {
                return Combine(V_21_Preamble, DCS);
            }
        }

        private void HandlePassThroughFaxPacket(SipDialog dialog, byte[] packet)
        {
            dialog.PassThroughFaxStream.Write(packet, RtpHeaderLength, packet.Length - RtpHeaderLength);
        }

        public void TransceiveFax(SipDialog dialog)
        {
            Enable_logging(1);

            int Chunk_size = 800;
            int chunkIndex = 0;
            int transceiveResult = 0;
            byte[] ALAW_Input = new Byte[Chunk_size];
            Int16[] PCM_Input = new Int16[Chunk_size];

            Int16[] PCM_Output = new Int16[Chunk_size];
            byte[] ALAW_Output = new byte[Chunk_size];

            byte[] state_log = new byte[20];

            // Wait...  In this interval dll put some samples in PCM output buffer
            System.Threading.Thread.Sleep(100);

            Logger.WriteImportant("TransceiveFax: started...");

            while (true)
            {
                while ((dialog.PassThroughFaxStream.Length - Initial_Fax_Chunk_size - (chunkIndex * Chunk_size)) < Chunk_size)
                    System.Threading.Thread.Sleep(1);
                //System.Threading.Thread.Sleep(120);

                ALAW_Input = dialog.PassThroughFaxStream.ToArray().Skip(Initial_Fax_Chunk_size + chunkIndex * Chunk_size).Take(Chunk_size).ToArray();
                chunkIndex++;


                g711_decode(type, PCM_Input, ALAW_Input, Chunk_size);


                //do
                //{
                transceiveResult = FAX_transceiver(PCM_Output, PCM_Input, Chunk_size, dialog.CallerID, state_log);
                Logger.WriteInfo("Z2 {0}: {1}", dialog.CallerID, FormatDSPLog(state_log));
                //Logger.WriteImportant("len: {0}, index: {1}", dialog.PassThroughFaxStream.Length.ToString(), chunkIndex.ToString());
                //System.Threading.Thread.Sleep(10);
                //} while (transceiveResult == -1);


                g711_encode(type, ALAW_Output, PCM_Output, Chunk_size);
                //Logger.WriteImportant(string.Join("", state_log));


                for (int i = 0; i < 5; i++)
                {
                    SendVoiceChunkStream(dialog, ALAW_Output.Skip(i * RtpChunkSize).Take(RtpChunkSize).ToArray());
                }
                //PlayVoice(dialog, ALAW_Output);
            }

            //int chunks = 480000 / Chunk_size;
            //for (int chunk_nu = 0; chunk_nu < chunks - 5; chunk_nu++)
            //{
            //    for (int i = 0; i < Chunk_size; i++)
            //    {
            //        PassThroughFAXInitialStream.Position = Initial_Fax_Chunk_size + chunk_nu * Chunk_size + i;
            //        ALAW_Input[i] = (byte)PassThroughFAXInitialStream.ReadByte();
            //    }

            //    g711_decode(type, PCM_Input, ALAW_Input, Chunk_size);
            //    FAX_transceiver(PCM_Output, PCM_Input, Chunk_size);

            //    byte[] pcm_byte_stream = new byte[Chunk_size * sizeof(Int16)];
            //    Buffer.BlockCopy(PCM_Output, 0, pcm_byte_stream, 0, pcm_byte_stream.Length);

            //    byte[] output = new byte[PCM_Output.Length];
            //    g711_encode(0, output, PCM_Output, PCM_Output.Length);

            //    PlayVoice(dialog, output);
            //    // wait for rtp samples... Input_LEN: 800/8000=100 ms.
            //    //System.Threading.Thread.Sleep(100);
            //    //
            //}
        }

        private void HandleFaxUdptl(SipDialog dialog, byte[] packet)
        {
            //if (UMSV.Schema.Config.Default.LogRtp)
            //    Logger.WriteView("Fax RTP {0}: {1}", DateTime.Now.Millisecond, String.Concat(Array.ConvertAll(packet, x => x.ToString("X2"))));

            //var seq = BitConverter.ToInt16(packet, 0);
            //var ifpSize = packet[2];


            //T30Indicator indicator = (T30Indicator)packet[15];

            //Logger.WriteImportant("packet bytes: {0}", packet.Length);
            //string bytes = string.Empty;
            //foreach (byte num in packet)
            //{
            //    bytes += num.ToString() + " - ";
            //}
            //Logger.WriteImportant(packet[45] + " *** " + bytes);
            //Logger.WriteStart("-----------------------------------------------------------------------");

            //switch (indicator)
            //{
            //    case T30Indicator.NoSignal:
            //        Logger.WriteTodo("NoSignal Received...");
            //        var response = new byte[] { 0, 0, 1, 2, 0, 0 };
            //        dialog.RtpNet.Send(response);
            //        break;
            //    case T30Indicator.CNG:
            //        Logger.WriteTodo("CNG Received...");
            //        break;

            //    //The same as v21_preamble
            //    case T30Indicator.CED:
            //        Logger.WriteTodo("CED / v21_preamble Received...");

            //        break;

            //    case T30Indicator.v27_2400_training:
            //        Logger.WriteTodo("v27_2400_training Received...");
            //        break;
            //    case T30Indicator.v27_4800_training:
            //        Logger.WriteTodo("v27_4800_training Received...");
            //        break;
            //    case T30Indicator.v29_7200_training:
            //        Logger.WriteTodo("v29_7200_training Received...");
            //        break;
            //    case T30Indicator.v29_9600_training:
            //        Logger.WriteTodo("v29_9600_training Received...");
            //        break;
            //    case T30Indicator.v17_7200_short_training:
            //        Logger.WriteTodo("v17_7200_short_training Received...");
            //        break;
            //    case T30Indicator.v17_7200_long_training:
            //        Logger.WriteTodo("v17_7200_long_training Received...");
            //        break;
            //    case T30Indicator.v17_9600_short_training:
            //        Logger.WriteTodo("v17_9600_short_training Received...");
            //        break;
            //    case T30Indicator.v17_9600_long_training:
            //        Logger.WriteTodo("v17_9600_long_training Received...");
            //        break;
            //    case T30Indicator.v17_12000_short_training:
            //        Logger.WriteTodo("v17_12000_short_training Received...");
            //        break;
            //    case T30Indicator.v17_12000_long_training:
            //        Logger.WriteTodo("v17_12000_long_training Received...");
            //        break;
            //    case T30Indicator.v17_14400_short_training:
            //        Logger.WriteTodo("v17_14400_short_training Received...");
            //        break;
            //    case T30Indicator.v17_14400_long_training:
            //        Logger.WriteTodo("v17_14400_long_training Received...");
            //        break;
            //    case T30Indicator.v8_ansam:
            //        Logger.WriteTodo("v8_ansam Received...");
            //        break;
            //    case T30Indicator.v8_signal:
            //        Logger.WriteTodo("v8_signal Received...");
            //        break;
            //    case T30Indicator.v34_cntl_channel_1200:
            //        Logger.WriteTodo("v34_cntl_channel_1200 Received...");
            //        break;
            //    case T30Indicator.v34_pri_channel:
            //        Logger.WriteTodo("v34_pri_channel Received...");
            //        break;
            //    case T30Indicator.v34_CC_retrain:
            //        Logger.WriteTodo("v34_CC_retrain Received...");
            //        break;
            //    case T30Indicator.v33_12000_training:
            //        Logger.WriteTodo("v33_12000_training Received...");
            //        break;
            //    case T30Indicator.v33_14400_training:
            //        Logger.WriteTodo("v33_14400_training Received...");
            //        break;
            //    default:
            //        Logger.WriteTodo("Default Received : {0}", indicator.ToString());
            //        int sampleRate = 8000;
            //        sbyte[] buffer = new sbyte[20800];
            //        double amplitude = sbyte.MaxValue;
            //        double frequency = 2100;
            //        for (int n = 0; n < buffer.Length; n++)
            //        {
            //            buffer[n] = (sbyte)(amplitude * Math.Sin((2 * Math.PI * n * frequency) / sampleRate));
            //        }

            //        byte[] byteBuff = new byte[buffer.Length];
            //        for (int i = 0; i < byteBuff.Length; i++)
            //        {
            //            byteBuff[i] = Convert.ToByte(buffer[i]);
            //        }
            //        //dialog.RtpNet.Send(byteBuff);
            //        //SipService.Default.PlayVoice(dialog, byteBuff);


            //        //int sampleRate = 8000;
            //        //byte[] buffer = new byte[(int)(sampleRate * 2.6)];
            //        //double amplitude = 0.25 * byte.MaxValue;
            //        //double frequency = 2100;
            //        //for (int n = 0; n < buffer.Length; n++)
            //        //{
            //        //    buffer[n] = (byte)(amplitude * Math.Sin((2 * Math.PI * n * frequency) / sampleRate));
            //        //}
            //        //dialog.RtpNet.Send(buffer);
            //        break;
            //}

            //var error_recovery = BitConverter.ToInt16(packet, 4);
            //var secondary_ifp_packets = BitConverter.ToInt16(packet, 6);
        }

        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        private string FormatDSPLog(byte[] log)
        {
            string result = string.Empty;
            for (int i = 0; i < log.Length; i++)
                result += Convert.ToChar(log[i]);

            return result;
        }

        #region FSK Frequency
        public static byte[] WAVE_1850Hz_1PACK_SAMPLES = new byte[] { 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, 60, 181, 184, 6, 58 };

        public static byte[] WAVE_1850Hz_2PACKS_SAMPLES = new byte[] { 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, 60, 181, 184, 6, 58,
                                                                       225, 186, 145, 59, 13, 190, 183, 50, 61, 180, 185, 3, 58, 235, 186, 235, 58, 3, 185, 180, 61, 50, 183, 190, 13, 59,};

        public static byte[] WAVE_1850Hz_3PACKS_SAMPLES = new byte[] { 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, 60, 181, 184, 6, 58,
                                                                       225, 186, 145, 59, 13, 190, 183, 50, 61, 180, 185, 3, 58, 235, 186, 235, 58, 3, 185, 180, 61, 50, 183, 190, 13, 59,
                                                                       145, 186, 225, 58, 6, 184, 181, 60, 51, 182, 190, 15, 59, 157, 186, 241, 58, 4, 184, 138, 60, 48, 177, 191, 8, 59,};

        public static byte[] WAVE_1850Hz_4PACKS_SAMPLES = new byte[] { 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, 60, 181, 184, 6, 58,
                                                                       225, 186, 145, 59, 13, 190, 183, 50, 61, 180, 185, 3, 58, 235, 186, 235, 58, 3, 185, 180, 61, 50, 183, 190, 13, 59,
                                                                       145, 186, 225, 58, 6, 184, 181, 60, 51, 182, 190, 15, 59, 157, 186, 241, 58, 4, 184, 138, 60, 48, 177, 191, 8, 59,
                                                                       152, 186, 213, 58, 24, 187, 136, 63, 49, 176, 188, 10, 56, 132, 186, 113, 58, 29, 187, 143, 62, 54, 179, 188, 53, };

        public static byte[] WAVE_1850Hz_5PACKS_SAMPLES = new byte[] { 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, 60, 181, 184, 6, 58,
                                                                       225, 186, 145, 59, 13, 190, 183, 50, 61, 180, 185, 3, 58, 235, 186, 235, 58, 3, 185, 180, 61, 50, 183, 190, 13, 59,
                                                                       145, 186, 225, 58, 6, 184, 181, 60, 51, 182, 190, 15, 59, 157, 186, 241, 58, 4, 184, 138, 60, 48, 177, 191, 8, 59,
                                                                       152, 186, 213, 58, 24, 187, 136, 63, 49, 176, 188, 10, 56, 132, 186, 113, 58, 29, 187, 143, 62, 54, 179, 188, 53,
                                                                       56, 134, 186, 97, 58, 17, 187, 141, 62, 55, 178, 189, 52, 57, 131, 186, 107, 58, 107, 186, 131, 57, 52, 189, 178, 55, };

        public static byte[] WAVE_1850Hz_6PACKS_SAMPLES = new byte[] { 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, 60, 181, 184, 6, 58,
                                                                       225, 186, 145, 59, 13, 190, 183, 50, 61, 180, 185, 3, 58, 235, 186, 235, 58, 3, 185, 180, 61, 50, 183, 190, 13, 59,
                                                                       145, 186, 225, 58, 6, 184, 181, 60, 51, 182, 190, 15, 59, 157, 186, 241, 58, 4, 184, 138, 60, 48, 177, 191, 8, 59,
                                                                       152, 186, 213, 58, 24, 187, 136, 63, 49, 176, 188, 10, 56, 132, 186, 113, 58, 29, 187, 143, 62, 54, 179, 188, 53,
                                                                       56, 134, 186, 97, 58, 17, 187, 141, 62, 55, 178, 189, 52, 57, 131, 186, 107, 58, 107, 186, 131, 57, 52, 189, 178, 55,
                                                                       62, 141, 187, 17, 58, 97, 186, 134, 56, 53, 188, 179, 54, 62, 143, 187, 29, 58, 113, 186, 132, 56, 10, 188, 176, 49, };


        public static byte[] WAVE_1850Hz_7PACKS_SAMPLES = new byte[] { 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, 60, 181, 184, 6, 58,
                                                                       225, 186, 145, 59, 13, 190, 183, 50, 61, 180, 185, 3, 58, 235, 186, 235, 58, 3, 185, 180, 61, 50, 183, 190, 13, 59,
                                                                       145, 186, 225, 58, 6, 184, 181, 60, 51, 182, 190, 15, 59, 157, 186, 241, 58, 4, 184, 138, 60, 48, 177, 191, 8, 59,
                                                                       152, 186, 213, 58, 24, 187, 136, 63, 49, 176, 188, 10, 56, 132, 186, 113, 58, 29, 187, 143, 62, 54, 179, 188, 53,
                                                                       56, 134, 186, 97, 58, 17, 187, 141, 62, 55, 178, 189, 52, 57, 131, 186, 107, 58, 107, 186, 131, 57, 52, 189, 178, 55,
                                                                       62, 141, 187, 17, 58, 97, 186, 134, 56, 53, 188, 179, 54, 62, 143, 187, 29, 58, 113, 186, 132, 56, 10, 188, 176, 49,
                                                                       63, 136, 187, 24, 58, 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, };

        public static byte[] WAVE_1850Hz_8PACKS_SAMPLES = new byte[] { 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, 60, 181, 184, 6, 58,
                                                                       225, 186, 145, 59, 13, 190, 183, 50, 61, 180, 185, 3, 58, 235, 186, 235, 58, 3, 185, 180, 61, 50, 183, 190, 13, 59,
                                                                       145, 186, 225, 58, 6, 184, 181, 60, 51, 182, 190, 15, 59, 157, 186, 241, 58, 4, 184, 138, 60, 48, 177, 191, 8, 59,
                                                                       152, 186, 213, 58, 24, 187, 136, 63, 49, 176, 188, 10, 56, 132, 186, 113, 58, 29, 187, 143, 62, 54, 179, 188, 53, 
                                                                       56, 134, 186, 97, 58, 17, 187, 141, 62, 55, 178, 189, 52, 57, 131, 186, 107, 58, 107, 186, 131, 57, 52, 189, 178, 55, 
                                                                       62, 141, 187, 17, 58, 97, 186, 134, 56, 53, 188, 179, 54, 62, 143, 187, 29, 58, 113, 186, 132, 56, 10, 188, 176, 49, 
                                                                       63, 136, 187, 24, 58, 213, 186, 152, 59, 8, 191, 177, 48, 60, 138, 184, 4, 58, 241, 186, 157, 59, 15, 190, 182, 51, 
                                                                       60, 181, 184, 6, 58, 225, 186, 145, 59, 13, 190, 183, 50, 61, 180, 185, 3, 58, 235, 186, 235, 58, 3, 185, 180, 61, 50 };


        public static byte[] WAVE_1650Hz_1PACK_SAMPLES = new byte[] { 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190, 24, 58, 6, 191 };

        public static byte[] WAVE_1650Hz_2PACKS_SAMPLES = new byte[] { 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190, 24, 58, 6, 191,
                                                                       189, 15, 58, 107, 184, 182, 55, 59, 225, 186, 136, 50, 62, 132, 186, 132, 62, 50, 136, 186, 225, 59, 55, 182, 184, 107 };

        public static byte[] WAVE_1650Hz_3PACKS_SAMPLES = new byte[] { 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190, 24, 58, 6, 191,
                                                                       189, 15, 58, 107, 184, 182, 55, 59, 225, 186, 136, 50, 62, 132, 186, 132, 62, 50, 136, 186, 225, 59, 55, 182, 184, 107,
                                                                       58, 15, 189, 191, 6, 58, 24, 190, 179, 10, 58, 113, 187, 180, 49, 56, 145, 186, 141, 60, 60, 131, 186, 157, 57, 48 };

        public static byte[] WAVE_1650Hz_4PACKS_SAMPLES = new byte[] { 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190, 24, 58, 6, 191,
                                                                       189, 15, 58, 107, 184, 182, 55, 59, 225, 186, 136, 50, 62, 132, 186, 132, 62, 50, 136, 186, 225, 59, 55, 182, 184, 107,
                                                                       58, 15, 189, 191, 6, 58, 24, 190, 179, 10, 58, 113, 187, 180, 49, 56, 145, 186, 141, 60, 60, 131, 186, 157, 57, 48,
                                                                       181, 187, 213, 59, 53, 176, 185, 29, 58, 3, 188, 188, 13, 58, 17, 184, 177, 52, 59, 241, 186, 138, 51, 62, 152, 186 };

        public static byte[] WAVE_1650Hz_5PACKS_SAMPLES = new byte[] { 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190, 24, 58, 6, 191,
                                                                       189, 15, 58, 107, 184, 182, 55, 59, 225, 186, 136, 50, 62, 132, 186, 132, 62, 50, 136, 186, 225, 59, 55, 182, 184, 107,
                                                                       58, 15, 189, 191, 6, 58, 24, 190, 179, 10, 58, 113, 187, 180, 49, 56, 145, 186, 141, 60, 60, 131, 186, 157, 57, 48,
                                                                       181, 187, 213, 59, 53, 176, 185, 29, 58, 3, 188, 188, 13, 58, 17, 184, 177, 52, 59, 241, 186, 138, 51, 62, 152, 186,
                                                                       134, 63, 61, 143, 186, 235, 56, 54, 183, 187, 97, 58, 8, 178, 190, 4, 58, 4, 190, 178, 8, 58, 97, 187, 183, 54 };

        public static byte[] WAVE_1650Hz_6PACKS_SAMPLES = new byte[] { 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190, 24, 58, 6, 191,
                                                                       189, 15, 58, 107, 184, 182, 55, 59, 225, 186, 136, 50, 62, 132, 186, 132, 62, 50, 136, 186, 225, 59, 55, 182, 184, 107,
                                                                       58, 15, 189, 191, 6, 58, 24, 190, 179, 10, 58, 113, 187, 180, 49, 56, 145, 186, 141, 60, 60, 131, 186, 157, 57, 48,
                                                                       181, 187, 213, 59, 53, 176, 185, 29, 58, 3, 188, 188, 13, 58, 17, 184, 177, 52, 59, 241, 186, 138, 51, 62, 152, 186,
                                                                       134, 63, 61, 143, 186, 235, 56, 54, 183, 187, 97, 58, 8, 178, 190, 4, 58, 4, 190, 178, 8, 58, 97, 187, 183, 54,
                                                                       56, 235, 186, 143, 61, 63, 134, 186, 152, 62, 51, 138, 186, 241, 59, 52, 177, 184, 17, 58, 13, 188, 188, 3, 58, 29 };

        public static byte[] WAVE_1650Hz_7PACKS_SAMPLES = new byte[] { 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190, 24, 58, 6, 191,
                                                                       189, 15, 58, 107, 184, 182, 55, 59, 225, 186, 136, 50, 62, 132, 186, 132, 62, 50, 136, 186, 225, 59, 55, 182, 184, 107,
                                                                       58, 15, 189, 191, 6, 58, 24, 190, 179, 10, 58, 113, 187, 180, 49, 56, 145, 186, 141, 60, 60, 131, 186, 157, 57, 48,
                                                                       181, 187, 213, 59, 53, 176, 185, 29, 58, 3, 188, 188, 13, 58, 17, 184, 177, 52, 59, 241, 186, 138, 51, 62, 152, 186,
                                                                       134, 63, 61, 143, 186, 235, 56, 54, 183, 187, 97, 58, 8, 178, 190, 4, 58, 4, 190, 178, 8, 58, 97, 187, 183, 54,
                                                                       56, 235, 186, 143, 61, 63, 134, 186, 152, 62, 51, 138, 186, 241, 59, 52, 177, 184, 17, 58, 13, 188, 188, 3, 58, 29,
                                                                       185, 176, 53, 59, 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190 };

        public static byte[] WAVE_1650Hz_8PACKS_SAMPLES = new byte[] { 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190, 24, 58, 6, 191,
                                                                       189, 15, 58, 107, 184, 182, 55, 59, 225, 186, 136, 50, 62, 132, 186, 132, 62, 50, 136, 186, 225, 59, 55, 182, 184, 107,
                                                                       58, 15, 189, 191, 6, 58, 24, 190, 179, 10, 58, 113, 187, 180, 49, 56, 145, 186, 141, 60, 60, 131, 186, 157, 57, 48,
                                                                       181, 187, 213, 59, 53, 176, 185, 29, 58, 3, 188, 188, 13, 58, 17, 184, 177, 52, 59, 241, 186, 138, 51, 62, 152, 186,
                                                                       134, 63, 61, 143, 186, 235, 56, 54, 183, 187, 97, 58, 8, 178, 190, 4, 58, 4, 190, 178, 8, 58, 97, 187, 183, 54,
                                                                       56, 235, 186, 143, 61, 63, 134, 186, 152, 62, 51, 138, 186, 241, 59, 52, 177, 184, 17, 58, 13, 188, 188, 3, 58, 29,
                                                                       185, 176, 53, 59, 213, 187, 181, 48, 57, 157, 186, 131, 60, 60, 141, 186, 145, 56, 49, 180, 187, 113, 58, 10, 179, 190,
                                                                       24, 58, 6, 191, 189, 15, 58, 107, 184, 182, 55, 59, 225, 186, 136, 50, 62, 132, 186, 132, 62, 50, 136, 186, 225, 59 };
        #endregion
    }

    public enum T30Indicator
    {
        NoSignal = 0,
        CNG = 2,
        CED = 3,
        v21_preamble = 3,
        v27_2400_training = 4,
        v27_4800_training = 5,
        v29_7200_training = 6,
        v29_9600_training = 7,
        v17_7200_short_training = 8,
        v17_7200_long_training = 9,
        v17_9600_short_training = 10,
        v17_9600_long_training = 11,
        v17_12000_short_training = 12,
        v17_12000_long_training = 13,
        v17_14400_short_training = 14,
        v17_14400_long_training = 15,

        v8_ansam,
        v8_signal,
        v34_cntl_channel_1200,
        v34_pri_channel,
        v34_CC_retrain,
        v33_12000_training,
        v33_14400_training
    }

}
