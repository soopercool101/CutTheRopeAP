using System;
using System.Collections.Generic;
using CutTheRope.game;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.iframework.core
{
    internal class ResourceMgr : NSObject
    {
        public enum ResourceType
        {
            IMAGE,
            FONT,
            SOUND,
            BINARY,
            STRINGS,
            ELEMENT
        }

        public ResourceMgrDelegate resourcesDelegate;

        private Dictionary<int, NSObject> s_Resources = new Dictionary<int, NSObject>();

        private XMLNode xmlStrings;

        private int loaded;

        private int loadCount;

        private List<int> loadQueue = new List<int>();

        private int Timer;

        private bool bUseFake;

        public virtual bool hasResource(int resID)
        {
            NSObject value = null;
            s_Resources.TryGetValue(resID, out value);
            return value != null;
        }

        public virtual void addResourceToLoadQueue(int resID)
        {
            loadQueue.Add(resID);
            loadCount++;
        }

        public void clearCachedResources()
        {
            s_Resources = new Dictionary<int, NSObject>();
        }

        public virtual NSObject loadResource(int resID, ResourceType resType)
        {
            NSObject value = null;
            if (s_Resources.TryGetValue(resID, out value))
            {
                return value;
            }
            string path = ((resType != ResourceType.STRINGS) ? CTRResourceMgr.XNA_ResName(resID) : "");
            bool flag = false;
            float scaleX = getNormalScaleX(resID);
            float scaleY = getNormalScaleY(resID);
            if (flag)
            {
                scaleX = getWvgaScaleX(resID);
                scaleY = getWvgaScaleY(resID);
            }
            switch (resType)
            {
            case ResourceType.IMAGE:
                value = loadTextureImageInfo(path, null, flag, scaleX, scaleY);
                break;
            case ResourceType.SOUND:
                value = loadSoundInfo(path);
                break;
            case ResourceType.FONT:
                value = loadVariableFontInfo(path, resID, flag);
                s_Resources.Remove(resID);
                break;
            case ResourceType.STRINGS:
            {
                value = loadStringsInfo(resID);
                string text = value.ToString();
                value = NSS(text.Replace('\u00a0', ' '));
                break;
            }
            }
            if (value != null)
            {
                s_Resources.Add(resID, value);
            }
            return value;
        }

        public virtual NSObject loadSoundInfo(string path)
        {
            return new NSObject().init();
        }

        public NSString loadStringsInfo(int key)
        {
            key &= 0xFFFF;
            if (xmlStrings == null)
            {
                xmlStrings = XMLNode.parseXML("menu_strings.xml");
            }
            XMLNode xMLNode = null;
            try
            {
                xMLNode = xmlStrings.childs()[key];
            }
            catch (Exception)
            {
            }
            if (xMLNode != null)
            {
                string tag = "en";
                if (LANGUAGE == Language.LANG_RU)
                {
                    tag = "ru";
                }
                if (LANGUAGE == Language.LANG_FR)
                {
                    tag = "fr";
                }
                if (LANGUAGE == Language.LANG_DE)
                {
                    tag = "de";
                }
                XMLNode xMLNode2 = xMLNode.findChildWithTagNameRecursively(tag, false);
                if (xMLNode2 == null)
                {
                    xMLNode2 = xMLNode.findChildWithTagNameRecursively("en", false);
                }
                return xMLNode2.data;
            }
            return new NSString();
        }

        public virtual FontGeneric loadVariableFontInfo(string path, int resID, bool isWvga)
        {
            XMLNode xMLNode = XMLNode.parseXML(path);
            int num = xMLNode["charoff"].intValue();
            int num2 = xMLNode["lineoff"].intValue();
            int num3 = xMLNode["space"].intValue();
            XMLNode xMLNode2 = xMLNode.findChildWithTagNameRecursively("chars", false);
            XMLNode xMLNode3 = xMLNode.findChildWithTagNameRecursively("kerning", false);
            NSString data = xMLNode2.data;
            if (xMLNode3 != null)
            {
                NSString datum = xMLNode3.data;
            }
            FontGeneric fontGeneric = new Font().initWithVariableSizeCharscharMapFileKerning(data, (Texture2D)loadResource(resID, ResourceType.IMAGE), null);
            fontGeneric.setCharOffsetLineOffsetSpaceWidth(num, num2, num3);
            return fontGeneric;
        }

        public virtual Texture2D loadTextureImageInfo(string path, XMLNode i, bool isWvga, float scaleX, float scaleY)
        {
            if (i == null)
            {
                i = XMLNode.parseXML(path);
            }
            int num = i["filter"].intValue();
            bool flag = (num & 1) == 1;
            int defaultAlphaPixelFormat = i["format"].intValue();
            string text = fullPathFromRelativePath(path);
            if (flag)
            {
                Texture2D.setAntiAliasTexParameters();
            }
            else
            {
                Texture2D.setAliasTexParameters();
            }
            Texture2D.setDefaultAlphaPixelFormat((Texture2D.Texture2DPixelFormat)defaultAlphaPixelFormat);
            Texture2D texture2D = new Texture2D().initWithPath(text, true);
            if (texture2D == null)
            {
                throw new Exception("texture not found: " + text);
            }
            Texture2D.setDefaultAlphaPixelFormat(Texture2D.kTexture2DPixelFormat_Default);
            if (isWvga)
            {
                texture2D.setWvga();
            }
            texture2D.setScale(scaleX, scaleY);
            setTextureInfo(texture2D, i, isWvga, scaleX, scaleY);
            return texture2D;
        }

        public virtual void setTextureInfo(Texture2D t, XMLNode i, bool isWvga, float scaleX, float scaleY)
        {
            t.preCutSize = vectUndefined;
            XMLNode xMLNode = i.findChildWithTagNameRecursively("quads", false);
            if (xMLNode != null)
            {
                List<NSString> list = xMLNode.data.componentsSeparatedByString(',');
                if (list != null && list.Count > 0)
                {
                    float[] array = new float[list.Count];
                    for (int j = 0; j < list.Count; j++)
                    {
                        array[j] = list[j].floatValue();
                    }
                    setQuadsInfo(t, array, list.Count, scaleX, scaleY);
                }
            }
            XMLNode xMLNode2 = i.findChildWithTagNameRecursively("offsets", false);
            if (xMLNode2 == null)
            {
                return;
            }
            List<NSString> list2 = xMLNode2.data.componentsSeparatedByString(',');
            if (list2 == null || list2.Count <= 0)
            {
                return;
            }
            float[] array2 = new float[list2.Count];
            for (int k = 0; k < list2.Count; k++)
            {
                array2[k] = list2[k].floatValue();
            }
            setOffsetsInfo(t, array2, list2.Count, scaleX, scaleY);
            XMLNode xMLNode3 = i.findChildWithTagNameRecursively(NSS("preCutWidth"), false);
            XMLNode xMLNode4 = i.findChildWithTagNameRecursively(NSS("preCutHeight"), false);
            if (xMLNode3 != null && xMLNode4 != null)
            {
                t.preCutSize = vect(xMLNode3.data.intValue(), xMLNode4.data.intValue());
                if (isWvga)
                {
                    t.preCutSize.x /= 1.5f;
                    t.preCutSize.y /= 1.5f;
                }
            }
        }

        private static string fullPathFromRelativePath(string relPath)
        {
            return ContentFolder + relPath;
        }

        private void setQuadsInfo(Texture2D t, float[] data, int size, float scaleX, float scaleY)
        {
            int num = data.Length / 4;
            t.setQuadsCapacity(num);
            int num2 = -1;
            for (int i = 0; i < num; i++)
            {
                int num3 = i * 4;
                Rectangle rect = MakeRectangle(data[num3], data[num3 + 1], data[num3 + 2], data[num3 + 3]);
                if ((float)num2 < rect.h + rect.y)
                {
                    num2 = (int)ceil(rect.h + rect.y);
                }
                rect.x /= scaleX;
                rect.y /= scaleY;
                rect.w /= scaleX;
                rect.h /= scaleY;
                t.setQuadAt(rect, i);
            }
            if (num2 != -1)
            {
                t._lowypoint = num2;
            }
            t.optimizeMemory();
        }

        private void setOffsetsInfo(Texture2D t, float[] data, int size, float scaleX, float scaleY)
        {
            int num = size / 2;
            for (int i = 0; i < num; i++)
            {
                int num2 = i * 2;
                t.quadOffsets[i].x = data[num2];
                t.quadOffsets[i].y = data[num2 + 1];
                t.quadOffsets[i].x /= scaleX;
                t.quadOffsets[i].y /= scaleY;
            }
        }

        public virtual bool isWvgaResource(int r)
        {
            switch (r)
            {
            case 126:
            case 127:
            case 128:
            case 129:
            case 130:
            case 131:
            case 132:
            case 133:
            case 134:
            case 135:
            case 136:
                return false;
            default:
                return true;
            }
        }

        public virtual float getNormalScaleX(int r)
        {
            return 1f;
        }

        public virtual float getNormalScaleY(int r)
        {
            return 1f;
        }

        public virtual float getWvgaScaleX(int r)
        {
            return 1.5f;
        }

        public virtual float getWvgaScaleY(int r)
        {
            return 1.5f;
        }

        public virtual void initLoading()
        {
            loadQueue.Clear();
            loaded = 0;
            loadCount = 0;
        }

        public virtual int getPercentLoaded()
        {
            if (loadCount == 0)
            {
                return 100;
            }
            return 100 * loaded / getLoadCount();
        }

        public virtual void loadPack(int[] pack)
        {
            for (int i = 0; pack[i] != -1; i++)
            {
                addResourceToLoadQueue(pack[i]);
            }
        }

        public virtual void freePack(int[] pack)
        {
            for (int i = 0; pack[i] != -1; i++)
            {
                freeResource(pack[i]);
            }
        }

        public virtual void loadImmediately()
        {
            while (loadQueue.Count != 0)
            {
                int resId = loadQueue[0];
                loadQueue.RemoveAt(0);
                loadResource(resId);
                loaded++;
            }
        }

        public virtual void startLoading()
        {
            if (resourcesDelegate != null)
            {
                Timer = NSTimer.schedule(rmgr_internalUpdate, this, 1f / 45f);
            }
            bUseFake = loadQueue.Count < 100;
        }

        private int getLoadCount()
        {
            if (!bUseFake)
            {
                return loadCount;
            }
            return 100;
        }

        public void update()
        {
            if (loadQueue.Count > 0)
            {
                int resId = loadQueue[0];
                loadQueue.RemoveAt(0);
                loadResource(resId);
            }
            loaded++;
            if (loaded >= getLoadCount())
            {
                if (Timer >= 0)
                {
                    NSTimer.stopTimer(Timer);
                }
                Timer = -1;
                resourcesDelegate.allResourcesLoaded();
            }
        }

        private static void rmgr_internalUpdate(NSObject obj)
        {
            ((ResourceMgr)obj).update();
        }

        private void loadResource(int resId)
        {
            if (150 < resId)
            {
                return;
            }
            if (10 == resId)
            {
                if (xmlStrings == null)
                {
                    xmlStrings = XMLNode.parseXML("menu_strings.xml");
                }
                return;
            }
            if (isSound(resId))
            {
                Application.sharedSoundMgr().getSound(resId);
                return;
            }
            if (isFont(resId))
            {
                Application.getFont(resId);
                return;
            }
            try
            {
                Application.getTexture(resId);
            }
            catch (Exception)
            {
            }
        }

        public virtual void freeResource(int resId)
        {
            if (150 < resId)
            {
                return;
            }
            if (10 == resId)
            {
                xmlStrings = null;
                return;
            }
            if (isSound(resId))
            {
                Application.sharedSoundMgr().freeSound(resId);
                return;
            }
            NSObject value = null;
            if (s_Resources.TryGetValue(resId, out value))
            {
                if (value != null)
                {
                    value.dealloc();
                }
                s_Resources.Remove(resId);
            }
        }
    }
}
