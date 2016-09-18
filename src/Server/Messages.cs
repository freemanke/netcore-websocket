using System;

namespace Server
{
    enum MessageType
    {
        Login = 100,
        Text = 101,
        Image = 102,
        Voice = 103,
        Video = 104,
        Location = 105,
        Link = 106,

        Notify = 500,
        NotLoginNotify = 501,
        AccessTokenErrorNotify = 502,
    }

    public class BaseMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class LoginMessage : BaseMessage
    {
        public string UserName { get; set; }
        public string AccessToken { get; set; }
    }
 
    public class TextMessage : BaseMessage
    {
        public string Content { get; set; }
    }

    public class ImageMessage : BaseMessage
    {
        public string MediaId { get; set; } // 媒体id，通过该id从媒体服务器拉取数据
        public string Url { get; set; } // 图片链接地址
    }


    public class VoiceMessage : BaseMessage
    {
        public string MediaId { get; set; } // 媒体id，通过该id从媒体服务器获取媒体
        public string Format { get; set; } // 音频格式，例如：amr
    }

    public class VideoMessage : BaseMessage
    {
        public string MediaId { get; set; } // 媒体id，通过该id从媒体服务器获取媒体
        public string ThumbMediaId { get; set; } // 视频缩略图媒体id
    }

    public class LocationMessage : BaseMessage
    {
        double X { get; set; } // 地理位置纬度
        double Y { get; set; } // 地理位置经度
        public int Scale { get; set; } // 地图缩放大小
        public string Label { get; set; } // 地理位置信息
    }

    public class LinkMessage : BaseMessage
    {
        public string Title { get; set; } // 消息标题
        public string Description { get; set; } // 消息描述
        public string Url { get; set; } // 消息连接
    }

}