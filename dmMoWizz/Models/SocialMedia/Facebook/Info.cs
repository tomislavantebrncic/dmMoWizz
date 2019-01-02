using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.SocialMedia.Facebook
{
    public class Info
    {
        public dynamic jsonObj { get; set; }

        public Info(dynamic json)
        {
            jsonObj = json;

            Id = jsonObj.id;
            Name = jsonObj.name;
            Likes = new Likes(jsonObj.likes);
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Likes Likes { get; set; }
    }

    public class Likes
    {
        public dynamic jsonObj { get; set; }
        public Likes(dynamic json)
        {
            jsonObj = json;

            Data = new List<Like>();
            //Data = new Like[jsonObj.data.Length];

            for (int i = 0; i < jsonObj.data.Length; i++)
            {
                if (jsonObj.data[i].category.Equals("Movie"))
                {
                    Data.Add(new Like(jsonObj.data[i]));
                }
                //Data[i] = new Like(jsonObj.data[i]);
            }
        }

        public List<Like> Data { get; set; }
    }

    public class Like
    {
        public dynamic jsonObj { get; set; }

        public Like(dynamic json)
        {
            jsonObj = json;

            Id = jsonObj.id;
            Name = jsonObj.name;
            Category = jsonObj.category;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
    }
}