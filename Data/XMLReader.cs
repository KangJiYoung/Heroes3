﻿using Heroes3.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Heroes3.Data
{
    public static class XMLReader
    {
        public static UnitAnimation Read(string xml)
        {
            var unitAnimation = new UnitAnimation();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(File.ReadAllText(xml));

            var spritesData = ReadSprites(xmlDoc);

            var animations = xmlDoc.SelectNodes("/SpriteMapping/Animations/Animation");
            foreach (XmlNode animation in animations)
            {
                var animationType = (AnimationType)Enum.Parse(typeof(AnimationType), animation.Attributes["Name"].Value);
                var animationFrames = animation.FirstChild.ChildNodes;

                var frameSprites = new List<Rectangle>();
                foreach (XmlNode frame in animationFrames)
                {
                    var idSprite = int.Parse(frame.Attributes["SpriteId"].Value);

                    frameSprites.Add(spritesData[idSprite]);
                }

                unitAnimation.Animations.Add(animationType, frameSprites);
            }

            return unitAnimation;
        }

        public static IDictionary<CursorType, Rectangle> ReadCursorTypes(string xml)
        {
            var cursorTypes = new Dictionary<CursorType, Rectangle>();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(File.ReadAllText(xml));

            var spritesData = ReadSprites(xmlDoc);

            var animations = xmlDoc.SelectNodes("/SpriteMapping/Animations/Animation");
            foreach (XmlNode animation in animations)
            {
                var cursorType = (CursorType)Enum.Parse(typeof(CursorType), animation.Attributes["Name"].Value);
                var animationFrame = animation.FirstChild.ChildNodes.Item(0);
                var idSprite = int.Parse(animationFrame.Attributes["SpriteId"].Value);

                cursorTypes.Add(cursorType, spritesData[idSprite]);
            }

            return cursorTypes;
        }

        private static Dictionary<int, Rectangle> ReadSprites(XmlDocument xmlDoc)
        {
            var spritesData = new Dictionary<int, Rectangle>();
            var sprites = xmlDoc.SelectNodes("/SpriteMapping/Sprites/Sprite");
            foreach (XmlNode sprite in sprites)
            {
                var id = int.Parse(sprite.Attributes["Id"].Value);

                var spriteCoordinates = sprite.FirstChild.ChildNodes;

                var x = int.Parse(spriteCoordinates[0].FirstChild.Value);
                var y = int.Parse(spriteCoordinates[1].FirstChild.Value);
                var width = int.Parse(spriteCoordinates[2].FirstChild.Value);
                var height = int.Parse(spriteCoordinates[3].FirstChild.Value);

                spritesData.Add(id, new Rectangle(x, y, width, height));
            }

            return spritesData;
        }
    }
}
