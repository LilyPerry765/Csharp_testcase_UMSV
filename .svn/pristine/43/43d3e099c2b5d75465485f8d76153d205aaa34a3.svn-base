using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Folder;
using Enterprise;

namespace UMSV.Schema
{
    public partial class MediaGatewayConfig
    {
        public static MediaGatewayConfig Default;

        static MediaGatewayConfig()
        {
            UMSV.Schema.MediaGatewayConfig.Load();
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(Constants.MediaGatewayConfigFileName))
                {
                    Default = new MediaGatewayConfig();
                    Save();
                }

                string config = File.ReadAsUnicodeText(Constants.MediaGatewayConfigFileName);
                Default = GatewayConfigUtility.Deserialize<MediaGatewayConfig>(config);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(Constants.MediaGatewayConfigFileName + " file not found or error deserializing it", ex);
            }
        }

        public static bool Save()
        {
            try
            {
                string config = GatewayConfigUtility.Serialize<MediaGatewayConfig>(Default, true);
                File.WriteAsUnicodeText(Constants.MediaGatewayConfigFileName, config);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }
    }
}
