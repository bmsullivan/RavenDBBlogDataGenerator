using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Faker;
using Raven.Client.Document;

namespace RavenDBBlogDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var docStore = new DocumentStore()
            {
                Url = "http://localhost:8080"
            };

            docStore.Initialize();

            using (var session = docStore.OpenSession())
            using(var scope = new TransactionScope())
            {
                var blogIds = new List<string>();
                for (int i = 0; i < 100; i++)
                {
                    var blog = new Blog() {Id = string.Format("blogs/{0}", i+1), Author = Faker.Name.FullName(), Name = Lorem.Sentence()};
                    session.Store(blog);
                    blogIds.Add(blog.Id);
                }

                var rand = new Random();
                foreach (var blogId in blogIds)
                {
                    var numberOfPosts = rand.Next(50, 100);
                    for (int j = 0; j < numberOfPosts; j++)
                    {
                        var numberOfComments = rand.Next(0, 10);
                        var numberOfTags = rand.Next(1, 5);
                        var post = new Post
                        {
                            BlogId = blogId,
                            Content = string.Join("<br/><br/>", Faker.Lorem.Paragraphs(3)),
                            Title = Lorem.Sentence(),
                            Comments = new List<Comment>(),
                            Tags = new List<string>()
                        };

                        for (int k = 0; k < numberOfComments; k++)
                        {
                            post.Comments.Add(new Comment{CommenterName = Name.FullName(), Text = Lorem.Sentence()});
                        }

                        for (int m = 0; m < numberOfTags; m++)
                        {
                            post.Tags.Add(Lorem.Words(1).First());
                        }

                        session.Store(post);
                    }
                }

                session.SaveChanges();

                scope.Complete();
            }
        }
    }

    public class Blog
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
    }

    public class Post
    {
        public string Id { get; set; }
        public string BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<Comment> Comments { get; set; }
        public List<string> Tags { get; set; }
    }

    public class Comment
    {
        public string CommenterName { get; set; }
        public string Text { get; set; }
    }

}
