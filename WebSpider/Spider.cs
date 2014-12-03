using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Gurock.SmartInspect;
using System.Collections.Generic;
using HelperFunctions;

namespace WebSpider
{
    // HTTP Method: GET, POST, or POST with files/attachments
    public enum HTTPRequestType
    {
        Get,
        Post,
        MultipartPost
    }

    public class Spider
    {
        private static readonly object _cookieLocker = new object();

        private HttpWebRequest req;
        private HttpWebResponse rsp;
        private HttpWebResponse errorRsp;
        private string rspText;
        private string errorrspText;
        private MIMEPayload mimePayload;
        private CookieCollection cookies;
        private bool persistCookies;
        private string postVars;
        //SpiderCacheMapper spiderCacheMapper;
        SpiderCacheRecord spiderCacheRecord;

        public string PostVars
        {
            get
            {
                return postVars;
            }
            set
            {
                postVars = value;
            }
        }

        /// <summary>
        /// The URL of the next request. This must be set before any other parameters.
        /// </summary>
        public string Url
        {
            get
            {
                if (req == null)
                    return null;

                return req.RequestUri.OriginalString;
            }
            set
            {
                try
                {
                    SiAuto.Main.EnterMethod(this, "Url");
                    req = (HttpWebRequest)WebRequest.Create(value);
                    RequestObject.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                    req.KeepAlive = false;
                    req.Pipelined = false;
                    if (req.ServicePoint.ConnectionLimit != 5000)
                    {
                        req.ServicePoint.ConnectionLimit = 5000;
                    }
                    // This is to prevent the "417" status code error from web servers
                    // that don't support sending POST data in a follow-up request
                    req.ServicePoint.Expect100Continue = false;

                    // Don't allow 302 auto-redirects by default
                    req.AllowAutoRedirect = false;
                    req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.0.15) Gecko/2009101601 Firefox/3.0.15 (.NET CLR 3.5.30729)";
                    req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                }
                catch (UriFormatException uriEx)
                {
                    SiAuto.Main.LogException("url: " + value, uriEx);
                    throw uriEx;
                }
                catch (Exception e)
                {
                    SiAuto.Main.LogException(e);
                    throw e;
                }
                finally
                {
                    SiAuto.Main.LeaveMethod(this, "Url");
                }
            }
        }

        public bool FollowRedirect
        {
            set
            {
                if (req == null)
                    throw new WebException("Set the URI first.");
                req.AllowAutoRedirect = value;
                if (req.AllowAutoRedirect)
                {
                    req.MaximumAutomaticRedirections = 5;
                }
            }
        }

        public string UserAgent
        {
            get
            {
                if (req == null)
                    return null;
                return req.UserAgent;
            }
            set
            {
                if (req == null)
                    throw new WebException("Set the URI first.");
                req.UserAgent = value;
            }
        }

        // Set the request type. This must be set before any arguments are added
        // to the request, but after the URL is set.
        public HTTPRequestType Type
        {
            set
            {
                try
                {
                    SiAuto.Main.EnterMethod(this, "Type");
                    if (req == null)
                        throw new WebException("Set the URI first.");

                    switch (value)
                    {
                        // GET request
                        case HTTPRequestType.Get:
                            mimePayload = null;
                            postVars = null;
                            req.Method = "GET";
                            break;
                        // POST request
                        case HTTPRequestType.Post:
                            mimePayload = null;
                            postVars = "";
                            req.Method = "POST";
                            req.ContentType = "application/x-www-form-urlencoded";
                            break;
                        // POST request with attachments
                        case HTTPRequestType.MultipartPost:
                            mimePayload = new MIMEPayload();
                            postVars = null;
                            req.Method = "POST";
                            req.ContentType = "multipart/form-data; boundary=" + mimePayload.Boundary;
                            break;
                    }
                }
                catch (Exception e)
                {
                    SiAuto.Main.LogException(e);
                    throw e;
                }
                finally
                {
                    SiAuto.Main.LeaveMethod(this, "Type");
                }
            }
        }

        /// <summary>
        /// Set to true to persist cookies between requests, false to discard cookies.
        /// Cookies will always be saved after a request so this flag only applies to whether
        /// the cookies are sent in the header of the next request (SendRequest()). If
        /// PersistCookies is set to false and a new request made, all cookies stored up to
        /// this point will be discarded.
        /// </summary>
        public bool PersistCookies
        {
            get
            {
                return persistCookies;
            }
            set
            {
                persistCookies = value;
            }
        }

        public bool CookieBushnellFix { get; set; }

        public bool ForceCookieBushnellFix { get; set; }

        public bool headerCookieFix { get; set; }

        //public bool IsCacheEnabled { get; set; }
        public bool IsCachingResultsEnabled { get; set; }

        /// <summary>
        /// Add an argument to a POST request.
        /// </summary>
        /// <param name="key">Argument name</param>
        /// <param name="value">Argument value</param>
        public void AddValue(string key, string value)
        {
            try
            {
                SiAuto.Main.EnterMethod(this, "AddValue");
                if (postVars != null)
                    postVars += ((postVars.Length > 0) ? "&" : "") + key + "=" + value;

                if (mimePayload != null)
                    mimePayload.AddValue(key, value);
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
                throw e;
            }
            finally
            {
                SiAuto.Main.LeaveMethod(this, "AddValue");
            }
        }

        /// <summary>
        /// Add a file to a multi-part POST request.
        /// </summary>
        /// <param name="key">Argument name</param>
        /// <param name="value">Filename</param>
        public void AddFile(string key, string value)
        {
            try
            {
                SiAuto.Main.EnterMethod(this, "AddFile");
                if (mimePayload == null)
                    throw new Exception("Must use MultipartPost method to add files.");

                mimePayload.AddFile(key, value);
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
                throw e;
            }
            finally
            {
                SiAuto.Main.LeaveMethod(this, "AddFile");
            }
        }

        /// Send the currently pending request.
        /// <returns>The HTTP response object</returns>
        public HttpWebResponse SendRequest()
        {
            //if (req.Method == "GET" && this.IsCacheEnabled)
            //{
            //    using (var scm = StorageMappers.SpiderCacheMapper())
            //    {
            //        spiderCacheRecord = scm.Get(this.Url);
            //    }
            //    if (spiderCacheRecord != null)
            //    {
            //        req.IfModifiedSince = spiderCacheRecord.LastModified;
            //        if (!String.IsNullOrEmpty(spiderCacheRecord.ETag))
            //            req.Headers.Add("If-None-Match", spiderCacheRecord.ETag);
            //    }
            //}
            try
            {
                SiAuto.Main.EnterMethod(this, "SendRequest");
                Stream st = null;
                rsp = null;
                rspText = null;
                errorRsp = null;
                errorrspText = null;
                // Add previously stored cookies to the header if PersistCookies is on 
                if (persistCookies)
                {
                    if ((req.Method == "POST" && CookieBushnellFix == true) || (req.Method == "GET" && CookieBushnellFix == false) || ForceCookieBushnellFix == true)
                    {
                        req.CookieContainer = null;
                        string Cookie = "";
                        lock (_cookieLocker)
                        {
                            foreach (Cookie c in cookies)
                            {
                                try
                                {
                                    if (Cookie != "")
                                        Cookie += ";";

                                    Cookie += c.Name + "=" + c.Value;
                                }
                                catch (Exception ex)
                                {
                                    SiAuto.Main.LogException(String.Format("cookie name: {0},  cookie value: {1}, URL:{2}", c.Name, c.Value, req != null && req.RequestUri != null ? req.RequestUri.OriginalString : "null"), ex);
                                }
                            }
                        }
                        req.Headers.Set("Cookie", Cookie);
                    }
                    else
                    {
                        req.CookieContainer = new CookieContainer();

                        lock (_cookieLocker)
                        {
                            foreach (Cookie c in cookies)
                            {
                                if (c.Domain == "")
                                {
                                    c.Domain = req.RequestUri.Host;
                                }

                                c.Domain = "." + c.Domain.TrimStart(new char[] { '.' });

                                req.CookieContainer.Add(c);
                            }
                        }
                    }
                }
                // Otherwise discard all previous cookies
                else
                    cookies = new CookieCollection();

                // Create the request payload
                byte[] data = null;

                if (req.Method == "GET")
                    data = new byte[0];

                if (req.Method == "POST" && mimePayload == null)
                    data = Encoding.ASCII.GetBytes(postVars);

                if (req.Method == "POST" && mimePayload != null)
                {
                    mimePayload.Finish();
                    data = mimePayload.Data;
                }

                // Send the request payload if there is one (not applicable for GET requests)
                if (data.Length > 0)
                {
                    try
                    {
                        if (req.ContentLength != data.Length)
                            req.ContentLength = data.Length;
                        st = req.GetRequestStream();
                        st.Write(data, 0, data.Length);
                    }
                    catch (WebException ex)
                    {
                        SiAuto.Main.LogException("Spider_SendRequest" + this.Url, ex);
                    }
                    finally
                    {
                        if (st != null)
                            st.Close();
                    }
                }

                // Try to get the HTTP response
                try
                {
                    rsp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException ex)
                {
                    rsp = null;
                    errorRsp = (HttpWebResponse)ex.Response;
                    // Remember newly sent cookies
                    if (errorRsp != null)
                    {
                        foreach (Cookie c in errorRsp.Cookies)
                        {
                            if (NeedToAddThisCookie(cookies, c))
                                lock (_cookieLocker)
                                {
                                    cookies.Add(c);
                                }
                        }

                        if (errorRsp.Cookies.Count == 0 && headerCookieFix == true && errorRsp.Headers.ToString().ToLower().Contains("set-cookie"))
                        {
                            foreach (var cookie in Regex.Split(errorRsp.Headers["set-cookie"].Replace(", ", "-"), ","))
                            {
                                Cookie _newCookie = new Cookie();
                                var CookieDataSplit = Regex.Split(cookie, ";");
                                foreach (var c in CookieDataSplit)
                                {
                                    if (c.IsNullOrWhiteSpace())
                                        continue;

                                    string[] CookieLineData = new string[2];

                                    if (c.Contains("="))
                                    {
                                        CookieLineData[0] = c.Substring(0, c.IndexOf("="));
                                        CookieLineData[1] = c.Substring(c.IndexOf("=") + 1);
                                    }
                                    else
                                    {
                                        CookieLineData[0] = c;
                                    }
                                    if (CookieLineData[0].Trim().ToLower() == "path")
                                    {
                                        _newCookie.Path = CookieLineData[1];
                                    }
                                    else if (CookieLineData[0].Trim().ToLower() == "domain")
                                    {
                                        _newCookie.Domain = CookieLineData[1];
                                    }
                                    else if (CookieLineData[0].Trim().ToLower() == "expires")
                                    {
                                        _newCookie.Expires = DateTime.Now.AddDays(30);
                                    }
                                    else if (CookieLineData[0].Trim().ToLower() == "secure")
                                    {
                                        _newCookie.Secure = true;
                                    }
                                    else if (CookieLineData[0].Trim().ToLower() == "version")
                                    {
                                        int tmp = 0;
                                        if (int.TryParse(CookieLineData[1], out tmp))
                                            _newCookie.Version = Convert.ToInt32(CookieLineData[1]);
                                    }
                                    else if (CookieLineData[0].Trim().ToLower() == "max-age")
                                    {
                                    }
                                    else if (CookieLineData[0].Trim().ToLower() == "httponly")
                                    {
                                    }
                                    else
                                    {
                                        _newCookie.Name = CookieLineData[0];
                                        _newCookie.Value = CookieLineData[1];
                                    }

                                    lock (_cookieLocker)
                                    {
                                        cookies.Add(_newCookie);
                                    }
                                }
                            }
                        }
                    }
                }

                // Remember newly sent cookies
                if (rsp != null)
                {
                    foreach (Cookie c in rsp.Cookies)
                    {
                        if (NeedToAddThisCookie(cookies, c))
                            lock (_cookieLocker)
                            {
                                cookies.Add(c);
                            }
                    }

                    if (rsp.Cookies.Count == 0 && headerCookieFix == true && rsp.Headers.ToString().ToLower().Contains("set-cookie") && rsp.Headers["set-cookie"] != null)
                    {
                        foreach (var cookie in Regex.Split(rsp.Headers["set-cookie"].Replace(", ", "-"), ","))
                        {
                            Cookie _newCookie = new Cookie();
                            var CookieDataSplit = Regex.Split(cookie, ";");
                            foreach (var c in CookieDataSplit)
                            {
                                if (c.IsNullOrWhiteSpace())
                                    continue;

                                string[] CookieLineData = new string[2];

                                if (c.Contains("="))
                                {
                                    CookieLineData[0] = c.Substring(0, c.IndexOf("="));
                                    CookieLineData[1] = c.Substring(c.IndexOf("=") + 1);
                                }
                                else
                                {
                                    CookieLineData[0] = c;
                                }
                                if (CookieLineData[0].Trim().ToLower() == "path")
                                {
                                    _newCookie.Path = CookieLineData[1];
                                }
                                else if (CookieLineData[0].Trim().ToLower() == "domain")
                                {
                                    _newCookie.Domain = CookieLineData[1];
                                }
                                else if (CookieLineData[0].Trim().ToLower() == "expires")
                                {
                                    _newCookie.Expires = DateTime.Now.AddDays(30);
                                }
                                else if (CookieLineData[0].Trim().ToLower() == "secure")
                                {
                                    _newCookie.Secure = true;
                                }
                                else if (CookieLineData[0].Trim().ToLower() == "version")
                                {
                                    _newCookie.Version = Convert.ToInt32(CookieLineData[1]);
                                }
                                else if (CookieLineData[0].Trim().ToLower() == "max-age")
                                {
                                }
                                else if (CookieLineData[0].Trim().ToLower() == "httponly")
                                {
                                }
                                else
                                {
                                    if (!CookieLineData[0].IsNullOrWhiteSpace())
                                        _newCookie.Name = CookieLineData[0].Trim();
                                    if (!CookieLineData[1].IsNullOrWhiteSpace())
                                        _newCookie.Value = CookieLineData[1];
                                }

                                if (NeedToAddThisCookie(cookies, _newCookie))
                                {
                                    lock (_cookieLocker)
                                    {
                                        if (cookies[_newCookie.Name] != null && cookies[_newCookie.Name].Value != "")
                                        {
                                            cookies.Add(new Cookie()
                                            {
                                                Name = cookies[_newCookie.Name].Name,
                                                Value = cookies[_newCookie.Name].Value,
                                                Expires = DateTime.Now.AddDays(-1)
                                            });
                                        }
                                        cookies.Add(_newCookie);
                                    }
                                }
                            }
                        }
                    }
                }

                // Return the response
                return rsp;
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
                throw e;
            }
            finally
            {
                SiAuto.Main.LeaveMethod(this, "SendRequest");
            }
        }

        private bool NeedToAddThisCookie(CookieCollection cookieTracker, Cookie c)
        {
            Cookie[] cookieArray = new Cookie[cookieTracker.Count];
            cookies.CopyTo(cookieArray, 0);

            string separator = "=";

            Dictionary<string, DateTime> cookieData = new Dictionary<string, DateTime>();
            foreach (var cc in cookieArray)
            {
                cookieData.AddSafely(cc.Name + separator + cc.Value, cc.Expires);
            }

            string cookieId = c.Name;

            if (cookieData.ContainsKey(cookieId))
            //if (cookieData.Keys.Where(w=>w.StartsWith(cookieId)).Any())
            {
                //TODO: there should be a code with expiration date

                return false;
            }
            return true;
        }

        public HttpWebRequest RequestObject
        {
            get
            {
                return req;
            }
        }

        public HttpWebResponse ResponseObject
        {
            get
            {
                return rsp;
            }
        }

        public HttpWebResponse ErrorResponseObject
        {
            get
            {
                return errorRsp;
            }
        }

        public string ErrorResponseText
        {
            get
            {
                try
                {
                    SiAuto.Main.EnterMethod(this, "ErrorResponseText");
                    if (errorrspText == null && errorRsp != null)
                    {
                        try
                        {
                            StreamReader sr = new StreamReader(errorRsp.GetResponseStream());
                            if (errorRsp.ContentEncoding.ToLower().Contains("gzip"))
                            {
                                sr = new StreamReader(new GZipStream(errorRsp.GetResponseStream(), CompressionMode.Decompress), Encoding.Default);
                            }
                            else if (errorRsp.ContentEncoding.ToLower().Contains("deflate"))
                            {
                                sr = new StreamReader(new DeflateStream(errorRsp.GetResponseStream(), CompressionMode.Decompress), Encoding.Default);
                            }
                            errorrspText = sr.ReadToEnd();
                            sr.Close();
                        }
                        catch (Exception ex)
                        {
                            SiAuto.Main.LogException("Spider", ex);
                            throw ex;
                        }
                    }
                    return errorrspText;
                }
                catch (Exception e)
                {
                    SiAuto.Main.LogException(e);
                    throw e;
                }
                finally
                {
                    SiAuto.Main.LeaveMethod(this, "ErrorResponseText");
                }
            }
        }

        ///// <summary>
        ///// Returns response result and update isNullBecauseOfSameCache parameter.
        ///// </summary>
        ///// <param name="isNullBecauseOfSameCache"></param>
        ///// <returns></returns>
        //public string GetResponseText(out bool isNullBecauseOfSameCache)
        //{
        //    isNullBecauseOfSameCache = false;

        //    if (rspText == null && rsp != null)
        //    {
        //        try
        //        {
        //            //if (req.Method == "GET" && this.IsCacheEnabled)
        //            //{
        //            //    if (rsp.StatusCode == HttpStatusCode.NotModified)
        //            //    {
        //            //        rspText = null;
        //            //        isNullBecauseOfSameCache = true;
        //            //    }
        //            //    else//if modified
        //            //    {
        //            //        StreamReader sr = new StreamReader(rsp.GetResponseStream());
        //            //        if (rsp.ContentEncoding.ToLower().Contains("gzip"))
        //            //        {
        //            //            sr = new StreamReader(new GZipStream(rsp.GetResponseStream(), CompressionMode.Decompress), Encoding.Default);
        //            //        }
        //            //        else if (rsp.ContentEncoding.ToLower().Contains("deflate"))
        //            //        {
        //            //            sr = new StreamReader(new DeflateStream(rsp.GetResponseStream(), CompressionMode.Decompress), Encoding.Default);
        //            //        }

        //            //        rspText = sr.ReadToEnd();
        //            //        sr.Close();

        //            //        if (spiderCacheRecord != null)
        //            //        {
        //            //            if (rsp.LastModified == spiderCacheRecord.LastModified)
        //            //            {
        //            //                rspText = null;
        //            //                isNullBecauseOfSameCache = true;
        //            //            }
        //            //            else if (rspText.CreateMD5Hash() == spiderCacheRecord.ResponseHash)
        //            //            {
        //            //                rspText = null;
        //            //                isNullBecauseOfSameCache = true;

        //            //                spiderCacheRecord.LastModified = rsp.LastModified;
        //            //                using (var scm = StorageMappers.SpiderCacheMapper())
        //            //                {
        //            //                    scm.UpdateRecord(spiderCacheRecord);
        //            //                }
        //            //            }
        //            //            else
        //            //            {
        //            //                spiderCacheRecord.LastModified = rsp.LastModified;
        //            //                spiderCacheRecord.ResponseHash = rspText.CreateMD5Hash();
        //            //                spiderCacheRecord.ETag = rsp.Headers["ETag"];
        //            //                using (var scm = StorageMappers.SpiderCacheMapper())
        //            //                {
        //            //                    scm.UpdateRecord(spiderCacheRecord);
        //            //                }
        //            //            }
        //            //        }
        //            //        else
        //            //        {
        //            //            spiderCacheRecord = new SpiderCacheRecord()
        //            //            {
        //            //                ETag = rsp.Headers["ETag"],
        //            //                LastModified = rsp.LastModified,
        //            //                ResponseHash = rspText.CreateMD5Hash(),
        //            //                URL = this.Url
        //            //            };
        //            //            using (var scm = StorageMappers.SpiderCacheMapper())
        //            //            {
        //            //                scm.Save(spiderCacheRecord);
        //            //            }
        //            //        }
        //            //    }
        //            //}
        //            //else
        //            //{
        //            StreamReader sr = new StreamReader(rsp.GetResponseStream());
        //            if (rsp.ContentEncoding.ToLower().Contains("gzip"))
        //            {
        //                sr = new StreamReader(new GZipStream(rsp.GetResponseStream(), CompressionMode.Decompress), Encoding.Default);
        //            }
        //            else if (rsp.ContentEncoding.ToLower().Contains("deflate"))
        //            {
        //                sr = new StreamReader(new DeflateStream(rsp.GetResponseStream(), CompressionMode.Decompress), Encoding.Default);
        //            }
        //            rspText = sr.ReadToEnd();
        //            sr.Close();
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            SiAuto.Main.LogException("Spider => GetResponseText ", ex);

        //            spiderCacheRecord = null;
        //            return null;
        //        }
        //    }

        //    spiderCacheRecord = null;

        //    return rspText;

        //}

        public string ResponseText
        {
            get
            {
                if (rspText == null && rsp != null)
                {
                    try
                    {
                        //if (req.Method == "GET" && this.IsCacheEnabled)
                        //{
                        //    if (rsp.StatusCode == HttpStatusCode.NotModified)
                        //    {
                        //        rspText = null;
                        //    }
                        //    else//if modified
                        //    {
                        //        StreamReader sr = new StreamReader(rsp.GetResponseStream());
                        //        if (rsp.ContentEncoding.ToLower().Contains("gzip"))
                        //        {
                        //            sr = new StreamReader(new GZipStream(rsp.GetResponseStream(), CompressionMode.Decompress), Encoding.Default);
                        //        }
                        //        else if (rsp.ContentEncoding.ToLower().Contains("deflate"))
                        //        {
                        //            sr = new StreamReader(new DeflateStream(rsp.GetResponseStream(), CompressionMode.Decompress), Encoding.Default);
                        //        }
                        //        rspText = sr.ReadToEnd();
                        //        sr.Close();
                        //        if (spiderCacheRecord != null)
                        //        {
                        //            if (rsp.LastModified == spiderCacheRecord.LastModified)
                        //            {
                        //                rspText = null;
                        //            }
                        //            //else if (RemoveFromCacheComparison != null && RemoveFromCacheComparison.Length > 0)
                        //            //{
                        //            //    if (rspText.ClearText().RemoveCharacters(RemoveFromCacheComparison, true) == spiderCacheRecord.ResponseHash.ClearText().RemoveCharacters(RemoveFromCacheComparison, true))
                        //            //    {
                        //            //        rspText = null;
                        //            //        spiderCacheRecord.LastModified = rsp.LastModified;
                        //            //        spiderCacheMapper.UpdateRecord(spiderCacheRecord);
                        //            //    }
                        //            //    else
                        //            //    {
                        //            //        spiderCacheRecord.LastModified = rsp.LastModified;
                        //            //        spiderCacheRecord.ResponseHash = rspText;
                        //            //        spiderCacheRecord.ETag = rsp.Headers["ETag"];
                        //            //        spiderCacheMapper.UpdateRecord(spiderCacheRecord);
                        //            //    }
                        //            //}
                        //            else if (rspText.CreateMD5Hash() == spiderCacheRecord.ResponseHash)
                        //            {
                        //                rspText = null;
                        //                spiderCacheRecord.LastModified = rsp.LastModified;
                        //                using (var scm = StorageMappers.SpiderCacheMapper())
                        //                {
                        //                    scm.UpdateRecord(spiderCacheRecord);
                        //                }
                        //            }
                        //            else
                        //            {
                        //                spiderCacheRecord.LastModified = rsp.LastModified;
                        //                spiderCacheRecord.ResponseHash = rspText.CreateMD5Hash();
                        //                spiderCacheRecord.ETag = rsp.Headers["ETag"];
                        //                using (var scm = StorageMappers.SpiderCacheMapper())
                        //                {
                        //                    scm.UpdateRecord(spiderCacheRecord);
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            spiderCacheRecord = new SpiderCacheRecord()
                        //            {
                        //                ETag = rsp.Headers["ETag"],
                        //                LastModified = rsp.LastModified,
                        //                ResponseHash = rspText.CreateMD5Hash(),
                        //                URL = this.Url
                        //            };
                        //            using (var scm = StorageMappers.SpiderCacheMapper())
                        //            {
                        //                scm.Save(spiderCacheRecord);
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        string charSet = rsp.CharacterSet;


                        Encoding encoding;
                        if (String.IsNullOrEmpty(charSet))
                            encoding = Encoding.Default;
                        else
                            encoding = Encoding.GetEncoding(charSet.Replace("'", "").Replace("\"", "").Replace("lias", ""));

                        StreamReader sr = new StreamReader(rsp.GetResponseStream(), encoding);
                        if (rsp.ContentEncoding.ToLower().Contains("gzip"))
                        {
                            sr = new StreamReader(new GZipStream(rsp.GetResponseStream(), CompressionMode.Decompress), encoding);
                        }
                        else if (rsp.ContentEncoding.ToLower().Contains("deflate"))
                        {
                            sr = new StreamReader(new DeflateStream(rsp.GetResponseStream(), CompressionMode.Decompress), encoding);
                        }
                        rspText = sr.ReadToEnd();

                        //// Check real charset meta-tag in HTML
                        //int CharsetStart = rspText.IndexOf("charset=");
                        //if (CharsetStart > 0)
                        //{
                        //    CharsetStart += 8;
                        //    int CharsetEnd = rspText.IndexOfAny(new[] { ' ', '\"', ';' }, CharsetStart);
                        //    string RealCharset =
                        //           rspText.Substring(CharsetStart, CharsetEnd - CharsetStart);

                        //    // real charset meta-tag in HTML differs from supplied server header???
                        //    if (RealCharset != charSet)
                        //    {
                        //        // get correct encoding
                        //        Encoding CorrectEncoding = Encoding.GetEncoding(RealCharset);

                        //        // reset stream position to beginning
                        //        memoryStream.Seek(0, SeekOrigin.Begin);

                        //        // reread response stream with the correct encoding
                        //        StreamReader sr2 = new StreamReader(memoryStream, CorrectEncoding);

                        //        strWebPage = sr2.ReadToEnd();
                        //        // Close and clean up the StreamReader
                        //        sr2.Close();
                        //    }
                        //}

                        sr.Close();
                        //}
                    }
                    catch (Exception ex)
                    {
                        SiAuto.Main.LogException(ex);
                        throw ex;
                    }
                }

                spiderCacheRecord = null;

                return rspText;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                try
                {
                    SiAuto.Main.EnterMethod(this, "StatusCode");
                    if (rsp != null)
                    {
                        return rsp.StatusCode;
                    }
                    else
                    {
                        return HttpStatusCode.SeeOther;
                    }
                }
                catch (Exception e)
                {
                    SiAuto.Main.LogException(e);
                    throw e;
                }
                finally
                {
                    SiAuto.Main.LeaveMethod(this, "StatusCode");
                }
            }
        }

        public CookieCollection Cookies
        {
            set
            {
                cookies = value == null ? new CookieCollection() : value;//TODO: ask Ilya
            }

            get
            {
                return cookies;
            }
        }

        public Spider()
        {
            req = null;
            rsp = null;
            rspText = null;
            errorRsp = null;
            errorrspText = null;
            mimePayload = null;
            persistCookies = true;
            cookies = new CookieCollection();
            headerCookieFix = false;
            //spiderCacheMapper = new SpiderCacheMapper(ClusterKeyResolver.GetInfrastructureDataPairs());
        }

        public byte[] ConvertNonSeekableStreamToByteArray(Stream NonSeekableStream)
        {
            try
            {
                SiAuto.Main.EnterMethod(this, "ConvertNonSeekableStreamToByteArray");
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[1024];
                int bytes;
                while ((bytes = NonSeekableStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytes);
                }
                byte[] output = ms.ToArray();
                return output;
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
                throw e;
            }
            finally
            {
                SiAuto.Main.LeaveMethod(this, "ConvertNonSeekableStreamToByteArray");
            }
        }
    }

    public class MIMEPayload
    {
        private const string boundary = "------------328523758298hjcwuie";
        private byte[] data = new byte[0];

        public void AddValue(string name, string value)
        {
            try
            {
                SiAuto.Main.EnterMethod(this, "AddValue");
                string text;
                text = "--" + boundary + "\r\n";
                text += "Content-Disposition: form-data; name=\"" + name + "\"\r\n\r\n";
                text += value + "\r\n";

                byte[] bytes = Encoding.ASCII.GetBytes(text);

                byte[] final = new byte[data.Length + bytes.Length];
                Buffer.BlockCopy(data, 0, final, 0, data.Length);
                Buffer.BlockCopy(bytes, 0, final, data.Length, bytes.Length);
                data = final;
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
                throw e;
            }
            finally
            {
                SiAuto.Main.LeaveMethod(this, "AddValue");
            }
        }

        public void AddFile(string name, string fileName)
        {
            try
            {
                SiAuto.Main.EnterMethod(this, "AddFile");
                string text;
                text = "--" + boundary + "\r\n";
                text += "Content-Disposition: form-data; name=\"" + name + "\"; filename=\"" + fileName + "\"\r\n";
                text += "Content-Type: application/octet-stream\r\n\r\n";

                byte[] textBytes = Encoding.ASCII.GetBytes(text);
                byte[] fileBytes;

                try
                {
                    fileBytes = File.ReadAllBytes(fileName);
                }
                catch (Exception ex)
                {
                    fileBytes = new byte[0];
                    throw ex;
                }

                byte[] final = new byte[data.Length + textBytes.Length + fileBytes.Length + 2];
                Buffer.BlockCopy(data, 0, final, 0, data.Length);
                Buffer.BlockCopy(textBytes, 0, final, data.Length, textBytes.Length);
                Buffer.BlockCopy(fileBytes, 0, final, data.Length + textBytes.Length, fileBytes.Length);
                final[final.Length - 2] = 13;
                final[final.Length - 1] = 10;
                data = final;
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
                throw e;
            }
            finally
            {
                SiAuto.Main.LeaveMethod(this, "AddFile");
            }
        }

        public void Finish()
        {
            try
            {
                SiAuto.Main.EnterMethod(this, "Finish");
                byte[] bytes = Encoding.ASCII.GetBytes("--" + boundary + "--");

                byte[] final = new byte[data.Length + bytes.Length];
                Buffer.BlockCopy(data, 0, final, 0, data.Length);
                Buffer.BlockCopy(bytes, 0, final, data.Length, bytes.Length);
                data = final;
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
                throw e;
            }
            finally
            {
                SiAuto.Main.LeaveMethod(this, "Finish");
            }
        }

        public string Boundary
        {
            get
            {
                return boundary;
            }
        }

        public byte[] Data
        {
            get
            {
                return data;
            }
        }
    }
}