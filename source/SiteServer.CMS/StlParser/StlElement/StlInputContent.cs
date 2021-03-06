﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "获取提交表单值", Description = "通过 stl:inputContent 标签在模板中显示指定表单的提交值")]
    public class StlInputContent
	{
        private StlInputContent() { }
        public const string ElementName = "stl:inputContent";

		public const string AttributeType = "type";
        public const string AttributeLeftText = "leftText";
        public const string AttributeRightText = "rightText";
        public const string AttributeFormatString = "formatString";
        public const string AttributeSeparator = "separator";
        public const string AttributeStartIndex = "startIndex";
        public const string AttributeLength = "length";
		public const string AttributeWordNum = "wordNum";
        public const string AttributeEllipsis = "ellipsis";
        public const string AttributeReplace = "replace";
        public const string AttributeTo = "to";
        public const string AttributeIsClearTags = "isClearTags";
        public const string AttributeIsReturnToBr = "isReturnToBr";
        public const string AttributeIsLower = "isLower";
        public const string AttributeIsUpper = "isUpper";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
	        {AttributeType, "显示的类型"},
	        {AttributeLeftText, "显示在信息前的文字"},
	        {AttributeRightText, "显示在信息后的文字"},
	        {AttributeFormatString, "显示的格式"},
	        {AttributeSeparator, "显示多项时的分割字符串"},
	        {AttributeStartIndex, "字符开始位置"},
	        {AttributeLength, "指定字符长度"},
	        {AttributeWordNum, "显示字符的数目"},
	        {AttributeEllipsis, "文字超出部分显示的文字"},
	        {AttributeReplace, "需要替换的文字，可以是正则表达式"},
	        {AttributeTo, "替换replace的文字信息"},
	        {AttributeIsClearTags, "是否清除标签信息"},
	        {AttributeIsReturnToBr, "是否将回车替换为HTML换行标签"},
	        {AttributeIsLower, "是否转换为小写"},
	        {AttributeIsUpper, "是否转换为大写"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
            if (contextInfo.ItemContainer?.InputItem == null) return string.Empty;
			try
			{
                var leftText = string.Empty;
                var rightText = string.Empty;
                var formatString = string.Empty;
                string separator = null;
                var startIndex = 0;
                var length = 0;
				var wordNum = 0;
                var ellipsis = StringUtils.Constants.Ellipsis;
                var replace = string.Empty;
                var to = string.Empty;
                var isClearTags = false;
                var isReturnToBr = false;
                var isLower = false;
                var isUpper = false;
                var type = string.Empty;
                var isDynamic = false;
                var attributes = new StringDictionary();

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            type = attr.Value.ToLower();
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLeftText))
                        {
                            leftText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeRightText))
                        {
                            rightText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeFormatString))
                        {
                            formatString = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSeparator))
                        {
                            separator = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeStartIndex))
                        {
                            startIndex = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLength))
                        {
                            length = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWordNum))
                        {
                            wordNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeEllipsis))
                        {
                            ellipsis = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeReplace))
                        {
                            replace = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTo))
                        {
                            to = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsClearTags))
                        {
                            isClearTags = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsReturnToBr))
                        {
                            isReturnToBr = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsLower))
                        {
                            isLower = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsUpper))
                        {
                            isUpper = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            attributes.Add(attr.Name, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, attributes, leftText, rightText, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, type);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, stlElement, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, StringDictionary attributes, string leftText, string rightText, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string type)
        {
            var parsedContent = string.Empty;

            if (InputContentAttribute.Id.ToLower().Equals(type))
            {
                parsedContent = SqlUtils.EvalString(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.Id);
            }
            else if (InputContentAttribute.AddDate.ToLower().Equals(type))
            {
                var addDate = SqlUtils.EvalDateTime(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.AddDate);
                parsedContent = DateUtils.Format(addDate, formatString);
            }
            else if (InputContentAttribute.UserName.ToLower().Equals(type))
            {
                parsedContent = SqlUtils.EvalString(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.UserName);
            }
            else if (InputContentAttribute.Reply.ToLower().Equals(type))
            {
                var content = SqlUtils.EvalString(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.Reply);
                parsedContent = content;
                parsedContent = StringUtils.ParseString(InputType.TextEditor, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
            }
            else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex))
            {
                parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.InputItem.ItemIndex, type, contextInfo).ToString();
            }
            else
            {
                var id = SqlUtils.EvalInt(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.Id);
                //var inputContentInfo = DataProvider.InputContentDao.GetContentInfo(id);
                var inputContentInfo = InputContent.GetContentInfo(id, pageInfo.Guid);
                if (inputContentInfo != null)
                {
                    parsedContent = inputContentInfo.GetExtendedAttribute(type);
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        if (!InputContentAttribute.HiddenAttributes.Contains(type.ToLower()))
                        {
                            var styleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, type, RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, pageInfo.PublishmentSystemId, inputContentInfo.InputId));
                            parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, separator, pageInfo.PublishmentSystemInfo, ETableStyle.InputContent, styleInfo, formatString, attributes, node.InnerXml, false);
                            parsedContent = StringUtils.ParseString(InputTypeUtils.GetEnumType(styleInfo.InputType), parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = leftText + parsedContent + rightText;
            }

            return parsedContent;
        }
	}
}
