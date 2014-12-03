using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HelperFunctions
{
    [Serializable()]
    public class QueryStringBuilder
    {
        private Dictionary<string, string> _dictionary;
        public virtual string this[string key]
        {
            get
            {
                key = CheckCase(key);
                if (_dictionary.ContainsKey(key)) return _dictionary[key];
                return String.Empty;
            }
            set
            {
                SetValue(key, value.ToString());
            }

        }
        public List<string> Keys()
        {
            return _dictionary.Keys.ToList();
        }
        public QueryStringBuilder()
        {
            _dictionary = new Dictionary<string, string>();
        }

        public QueryStringBuilder(NameValueCollection nameValueCollection)
        {
            _dictionary = new Dictionary<string, string>();
            foreach (string key in nameValueCollection.AllKeys)
            {
                if (!key.IsNullOrWhiteSpace())
                {
                    _dictionary.Add(key, nameValueCollection[key]);
                }
            }
        }

        public QueryStringBuilder(Uri uri)
        {
            _dictionary = new Dictionary<string, string>();
            if (!uri.Query.IsNullOrWhiteSpace())
            {
                string separator = "&amp;";
                if (uri.Query.IndexOf(separator) == -1) separator = "&";
                string[] queryParams = uri.Query.TrimStart('?').Split(separator.ToCharArray());
                if (!queryParams.IsNullOrEmpty())
                {
                    foreach (string qp in queryParams)
                    {
                        if (qp.IndexOf("=") >= 0)
                        {
                            string[] pair = qp.Split('=');
                            if (!pair.IsNullOrEmpty() && pair.Length == 2)
                            {
                                this.SetValue(HttpUtility.UrlDecode(pair[0].Trim()), pair[1].Trim());
                            }
                            pair = null;
                        }
                        else
                        {
                            this.SetValue(HttpUtility.UrlDecode(qp));
                        }
                    }
                    queryParams = null;
                }
            }
        }

        public QueryStringBuilder SetValue(string keyOnly)
        {
            keyOnly = CheckCase(keyOnly);
            if (!_dictionary.ContainsKey(keyOnly)) _dictionary.Add(keyOnly, string.Empty);
            return this;
        }

        public QueryStringBuilder SetValue(string key, string value, bool addEmptyValue = false)
        {
            key = CheckCase(key);
            if (value.IsNullOrWhiteSpace() && addEmptyValue == false)
            {
                Remove(key);
            }
            else
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = value;
                }
                else
                {
                    _dictionary.Add(key, value);
                }
            }
            return this;
        }

        public QueryStringBuilder Remove(string key)
        {
            key = CheckCase(key);
            if (_dictionary.ContainsKey(key)) _dictionary.Remove(key);
            return this;
        }

        /// <summary>
        /// Remove every keys exept
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public QueryStringBuilder RemoveExept(List<string> keys)
        {
            keys = keys.Select(key => this.CheckCase(key)).ToList();

            if (!keys.IsNullOrEmpty() && !this._dictionary.IsNullOrEmpty())
            {
                int index = 0;

                while (index < _dictionary.Count())
                {
                    if (!keys.Contains(_dictionary.ElementAt(index).Key))
                    {
                        this._dictionary.Remove(_dictionary.ElementAt(index).Key);
                        continue;
                    }

                    index++;
                }

                //foreach (string kvp in this._dictionary.Keys)
                //{
                //    if (!keys.Contains(kvp))
                //    {
                //        this._dictionary.Remove(kvp);
                //    }
                //}
            }

            return this;
        }

        public string GetValue(string key)
        {
            key = CheckCase(key);
            if (_dictionary.ContainsKey(key))
                return _dictionary[key];
            else
                return string.Empty;
        }

        private string CheckCase(string key)
        {
            foreach (string validKey in _dictionary.Keys)
            {
                if (validKey.Equals(key, StringComparison.InvariantCultureIgnoreCase)) return validKey;
            }
            return key;
        }

        public QueryStringBuilder Clone()
        {
            QueryStringBuilder qs = new QueryStringBuilder();
            foreach (KeyValuePair<string, string> pair in _dictionary)
            {
                qs._dictionary.Add(pair.Key, _dictionary[pair.Key]);
            }
            return qs;
        }

        public override string ToString()
        {
            return this.ToString(false);
        }

        public string ToString(bool htmlEncode)
        {
            if (_dictionary.Count == 0) return String.Empty;
            StringBuilder sb = new StringBuilder();
            bool addSeparator = false;
            string separator = htmlEncode ? "&amp;" : "&";

            sb.Append("?");
            foreach (KeyValuePair<string, string> pair in _dictionary)
            {
                if (addSeparator)
                {
                    sb.Append(separator);
                }
                else
                {
                    addSeparator = true;
                }
                if (!pair.Value.IsNullOrWhiteSpace()) sb.Append(pair.Key).Append("=").Append(htmlEncode ? HttpUtility.UrlEncode(pair.Value) : pair.Value);
                else sb.Append(pair.Key.AppendString("="));
            }
            return sb.ToString();
        }
    }
}
