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
            if (json != null)
            {
                jsonObj = json;

                Id = jsonObj.id;
                Name = jsonObj.name;
                Likes = new Likes(jsonObj.likes);
                Friends = new Friends(jsonObj.friends);
            }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Likes Likes { get; set; }
        public Friends Friends { get; set; }
    }

    public class Likes
    {
        public dynamic jsonObj { get; set; }
        public Likes(dynamic json)
        {
            if (json != null)
            {
                jsonObj = json;

                Data = new List<Like>();

                for (int i = 0; i < jsonObj.data.Length; i++)
                {
                    if (jsonObj.data[i].category.Equals("Movie"))
                    {
                        Data.Add(new Like(jsonObj.data[i]));
                    }
                }
            }
        }

        public List<Like> Data { get; set; }
    }

    public class Like
    {
        public dynamic jsonObj { get; set; }

        public Like(dynamic json)
        {
            if (json != null)
            {
                jsonObj = json;

                Id = jsonObj.id;
                Name = jsonObj.name;
                Category = jsonObj.category;
            }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
    }

    public class Friends
    {
        public dynamic jsonObj { get; set; }
        public Friends(dynamic json)
        {
            if (json != null)
            {
                jsonObj = json;

                Data = new List<Friend>();

                for (int i = 0; i < jsonObj.data.Length; i++)
                {
                    Data.Add(new Friend(jsonObj.data[i]));
                }
            }
        }

        public List<Friend> Data { get; set; }
    }

    public class Friend
    {
        public dynamic jsonObj { get; set; }

        public Friend(dynamic json)
        {
            if (json != null)
            {
                jsonObj = json;

                Id = jsonObj.id;
                Likes = new Likes(jsonObj.likes);
            }
        }

        public string Id { get; set; }
        public Likes Likes { get; set; }
    }
}