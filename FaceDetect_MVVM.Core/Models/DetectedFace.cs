﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Windows.UI.Xaml.Media;

namespace FaceDetect_MVVM.Core.Models
{
    public class DetectedFace
    {
        public string faceId { get; set; }
        public FaceRectangle faceRectangle { get; set; }
        public FaceAttributes faceAttributes { get; set; }
        public Brush brush { get; set; }
    }
    public class FaceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class HeadPose
    {
        public double pitch { get; set; }
        public double roll { get; set; }
        public double yaw { get; set; }
    }

    public class FacialHair
    {
        public double moustache { get; set; }
        public double beard { get; set; }
        public double sideburns { get; set; }
    }

    public class Emotion
    {
        public double anger { get; set; }
        public double contempt { get; set; }
        public double disgust { get; set; }
        public double fear { get; set; }
        public double happiness { get; set; }
        public double neutral { get; set; }
        public double sadness { get; set; }
        public double surprise { get; set; }
    }

    public class Blur
    {
        public string blurLevel { get; set; }
        public double value { get; set; }
    }

    public class Exposure
    {
        public string exposureLevel { get; set; }
        public double value { get; set; }
    }

    public class Noise
    {
        public string noiseLevel { get; set; }
        public double value { get; set; }
    }

    public class Makeup
    {
        public bool eyeMakeup { get; set; }
        public bool lipMakeup { get; set; }
    }

    public class Occlusion
    {
        public bool foreheadOccluded { get; set; }
        public bool eyeOccluded { get; set; }
        public bool mouthOccluded { get; set; }
    }

    public class HairColor
    {
        public string color { get; set; }
        public double confidence { get; set; }
    }

    public class Hair
    {
        public double bald { get; set; }
        public bool invisible { get; set; }
        public List<HairColor> hairColor { get; set; }
    }

    public class FaceAttributes
    {
        public double smile { get; set; }
        public HeadPose headPose { get; set; }
        public string gender { get; set; }
        public double age { get; set; }
        public FacialHair facialHair { get; set; }
        public string glasses { get; set; }
        public Emotion emotion { get; set; }
        public Blur blur { get; set; }
        public Exposure exposure { get; set; }
        public Noise noise { get; set; }
        public Makeup makeup { get; set; }
        public List<object> accessories { get; set; }
        public Occlusion occlusion { get; set; }
        public Hair hair { get; set; }

        /// <summary>
        /// Az emóció visszaadása egy különleges, propertyken végig haladó ciklussal
        /// </summary>
        public string getEmotion
        {
            get
            {
                var prop = this.emotion.GetType().GetProperties();
                PropertyInfo max = (PropertyInfo)prop.GetValue(0);
                foreach (var props in prop)
                {
                    if ((double)props.GetValue(this.emotion) >= (double)max.GetValue(this.emotion))
                    {
                        max = props;
                    }
                }
                return (max.Name);
            }
        }

        /// <summary>
        /// A haj szín kiválasztása a jelen lévő szín értékek közzül
        /// </summary>
        public string getHairColor
        {
            get
            {
                if (this.hair.hairColor.Count == 0)
                {
                    return "bald";
                }
                HairColor res = new HairColor();
                res.confidence = 0;
                foreach (var hair in this.hair.hairColor)
                {
                    if (hair.confidence > res.confidence)
                    {
                        res = hair;
                    }
                }

                return res.color;
            }
        }
    }

}
