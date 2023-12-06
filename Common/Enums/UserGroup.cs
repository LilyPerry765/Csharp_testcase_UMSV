using System.ComponentModel;

namespace UMSV
{
    public enum UserGroup:byte
    {
        [Description("مدیر سیستم")]
        Admin = 1,

        [Description("ناظر سیستم")]
        Supervisor = 2,
    }
}
