using System.Net;

namespace IP_Address_Capture_Portal
{
    internal class Program
    {
        public static string server_address_http = "";
        public static string server_address_https = "";

        public static List<String> list = new List<String>();
        public static Dictionary<IPAddress, string> user_agent = new Dictionary<IPAddress, string>();

        public static Dictionary<IPAddress, List<string>> links = new Dictionary<IPAddress, List<string>>();
        public static Dictionary<string, IPAddress> links_rev = new Dictionary<string, IPAddress>();


        public static Dictionary<string, string> link_shortened = new Dictionary<string, string>();
        public static Dictionary<string, string> link_shortened_rev = new Dictionary<string, string>();

        public static Dictionary<IPAddress, List<string>> visited_sites = new Dictionary<IPAddress, List<string>>();
        public static string prefixes = "";


        public static string Get_ngrock_addressAsync(bool https = false)
        {
            WebRequest request = WebRequest.Create("http://127.0.0.1:4040/api/tunnels");
            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string data = reader.ReadToEnd();
                string[] main_data = data.Split(':');
                if (https == false)
                {
                    data = main_data[5].Split(',')[0];
                    data = "http:" + data.Remove(data.Length - 1, 1) + '/';
                    return data.ToString();
                }
                else
                {
                    data = main_data[35].Split(',')[0];
                    data = "https:" + data.Remove(data.Length - 1, 1) + '/';
                    return data.ToString();
                }
            }
        }
        static void Main(string[] args)
        {
            server_address_http = Get_ngrock_addressAsync();
            server_address_https = Get_ngrock_addressAsync(true);


            Console.WriteLine(server_address_http);
            Console.WriteLine(server_address_https);

            HttpListener listener = new HttpListener();

            listener.Prefixes.Add(server_address_http);

            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;


                string responseString = "";

                string hello;


                if (request.Url.AbsolutePath.ToString() != "/favicon.ico")
                {
                    responseString = link_shortener(request);
                }

                else
                {
                    responseString = favicon_return();
                }

                response.ContentType = "text/html";

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
        }

        public static string favicon_return()
        {
            return "Temp";
        }

        public static string link_shortener(HttpListenerRequest request)
        {
            string response = "";
            string request_text = request.Url.PathAndQuery.ToString();

            if (request_text == "create_link")
            {
                return "Please Enter A Valid Web Address";
            }

            if (request_text == "/")
            {
                return File.ReadAllText("test.html");
            }

            if (request_text.StartsWith("/shrt_lnk/"))
            {
                request_text = request_text.Split('/')[2];
                if (link_shortened_rev.ContainsKey(request_text))
                {
                    string url = link_shortened_rev[request_text];
                    string return_data = File.ReadAllText("first.txt") + url + File.ReadAllText("last.txt");
                    response = return_data;
                }
                else
                {
                    response = "Shortened Link Not Found";
                }
            }

            else if (request_text.Length > 1)
            {
                string splitter = "/create_link";
                if (request_text.StartsWith(splitter))
                {
                    string url = request_text.Substring(splitter.Length, request_text.Length - splitter.Length);
                    if (url.StartsWith("http") == false || url.StartsWith("https") == false)
                    {
                        url = "http://" + url;
                    }

                    if (link_shortened.ContainsKey(url))
                    {
                        response = "<p><h1 style=font-family:courier;font-size:15pt;text-align:center;position:relative;top:40%;>" + "<br><a href=" + url + ">" + url + "</a><br>" + " --> " + "<br><a href=" + prefixes + "shrt_lnk/" + link_shortened[url].ToString() + ">" + prefixes + "shrt_lnk/" + link_shortened[url].ToString() + "</a><br></h1></p>";
                    }
                    else
                    {
                        int count = link_shortened.Count;
                        string random_alnu = "";

                        int i = 0;
                        Random rnd = new Random();

                        bool unique = false;

                        while (unique == false)
                        {
                            while (i < 4)
                            {
                                int selector = rnd.Next(0, 3);
                                if (selector == 0)
                                {
                                    int char_val = rnd.Next(48, 57);
                                    random_alnu = random_alnu + (char)(char_val);
                                }
                                if (selector == 1)
                                {
                                    int char_val = rnd.Next(65, 90);
                                    random_alnu = random_alnu + (char)(char_val);
                                }
                                else
                                {
                                    int char_val = rnd.Next(97, 122);
                                    random_alnu = random_alnu + (char)(char_val);
                                }
                                i++;
                            }
                            if (links_rev.ContainsKey(random_alnu))
                            {
                                unique = false;
                            }
                            else
                            {
                                unique = true;
                            }
                        }

                        response = "<p><h1 style=font-family:courier;font-size:15pt;text-align:center;position:relative;top:40%;>" + "<br><a href=" + url + ">" + url + "</a><br>" + " --> " + "<br><a href=" + prefixes + "shrt_lnk/" + random_alnu + ">" + prefixes + "shrt_lnk/" + random_alnu + "</a><br></h1></p>";

                        //response = "<h1 style=font-family:courier;font-size:15pt;text-align:center;position:relative;top:40%;>" + "<a href=" + url + ">" + url + "</a>" + " --> " + "<a href=" + prefixes + condition + "/" + random_alnu + ">" + prefixes +  condition.Substring(1, condition.Length -1) +  random_alnu +"</a></h1>";


                        links_rev.Add(random_alnu, request.RemoteEndPoint.Address);
                        link_shortened.Add(url, random_alnu);
                        link_shortened_rev.Add(random_alnu, url);
                        if (links.ContainsKey(request.RemoteEndPoint.Address))
                        {
                            links[request.RemoteEndPoint.Address].Add(random_alnu);
                        }
                        else
                        {
                            List<string> count_ls = new List<string>();
                            count_ls.Add(random_alnu);
                            links.Add(request.RemoteEndPoint.Address, count_ls);
                        }
                    }
                }
                else
                {
                    response = "Invaid Input";
                }
            }
            return response;
        }
    }
}
