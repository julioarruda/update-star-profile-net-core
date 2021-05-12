using System;
using RestSharp;
using RestSharp.Authenticators;
using System.Xml;
using System.IO;
using System.Text.Json;
using System.ServiceModel.Syndication;
using System.Linq;


namespace test
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("==================Atualização Perfil GitHub Star==========================");
            post post1 = GetYoutubeVideo();

            bool validacao = validaUrl(post1.post_url,args[0], args[1] );

            if(!validacao)
            {
                InsertPostStar(args[0], post1,args[1]);
            }
            else
            {
                Console.WriteLine("Video Já existe");
            }

            Console.WriteLine("==================Atualização Concluida==========================");

        }

        static bool validaUrl (string url, string token, string api_url)
        {
            var client = new RestClient(api_url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer "+ token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query \\n{contributions{\\n  url\\n  description\\n}}\\n\",\"variables\":{}}",
                    ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            
            bool resposta = response.Content.Contains(url);

            return resposta;
        }

        static post GetYoutubeVideo()
        {
            Console.WriteLine("==================Buscando Vídeo mais recente do Canal==========================");

            var url = "https://www.youtube.com/feeds/videos.xml?channel_id=UCnQzZNPePG3EZMj7Qg3D0Sw";
            using var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);

            var post = feed.Items.FirstOrDefault();

            post post1 = new post();
            post1.post_title = post.Title.Text;
            post1.post_description = post.Title.Text;
            post1.post_url = post.Links[0].Uri.AbsoluteUri;
            post1.post_date = post.PublishDate.DateTime.ToString("yyyy-MM-dd");
            post1.post_kind = "VIDEO_PODCAST";

            return post1;
        }

        static void InsertPostStar(string token, post post, string api_url)
        {
            var client = new RestClient(api_url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer "+ token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"mutation createContribution ($input: ContributionInput){\\n    createContribution (data: $input) {\\n        title\\n        url\\n        description\\n        date\\n    }\\n}\",\"variables\":{\"input\":{\"type\":\""+post.post_kind+"\",\"title\":\""+post.post_title+"\",\"url\":\""+post.post_url+"\",\"description\":\""+post.post_description+"\",\"date\":\""+post.post_date+"T11:16:00.047Z\"}}}",
                       ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine("Video Incluido");
        }
    }


    public class post
    {
        public string post_title { get; set; }
        public string post_description { get; set; }

        public string post_date { get; set; }

        public string post_url { get; set; }

        public string post_kind { get; set; }
    }
}
