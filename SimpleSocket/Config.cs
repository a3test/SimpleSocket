using System.Text;

namespace SimpleSocket
{
    public class Config
    {
        /// <summary>
        /// 数据长度指示位的长度
        /// </summary>
        public const int ByteSizeLength = 4;

        /// <summary>
        /// 默认传输编码
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    }
}