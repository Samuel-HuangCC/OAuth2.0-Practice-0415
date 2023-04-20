using Azure.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;

namespace OAuth2._0_Practice_0415.Model
{
    public static class DbModel
    {
        public static OUser GetUser(string id)
        {
            var user = default(OUser);
            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select name,email,access_token,refresh_token,id_token,referenceid from oUser where id = @sub";
                        cmd.Parameters.Add(new SqlParameter("@sub", id));
                        cmd.Connection.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                user = new OUser()
                                {
                                    Id = id,
                                    Name = read["name"].ToString(),
                                    Email = read["email"].ToString(),
                                    AccessToken = read["access_token"].ToString(),
                                    RefreshToken = read["refresh_token"].ToString(),
                                    IdToken = read["id_token"].ToString(),
                                    ReferenceId = read["referenceid"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return user!;
        }

        public static List<OUser> GetAllUser()
        {
            var users = new List<OUser>();
            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select id,name,email,access_token,refresh_token,id_token,referenceid from oUser";
                        cmd.Connection.Open();
                        using (var read = cmd.ExecuteReader())
                        {
                            while (read.Read())
                            {
                                users.Add(new OUser()
                                {
                                    Id = read["id"].ToString(),
                                    Name = read["name"].ToString(),
                                    Email = read["email"].ToString(),
                                    AccessToken = read["access_token"].ToString(),
                                    RefreshToken = read["refresh_token"].ToString(),
                                    IdToken = read["id_token"].ToString(),
                                    ReferenceId = read["referenceid"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return users!;
        }


        public static int InsertUser(OUser user)
        {
            var result = 0;
            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "insert into oUser(id,name,email,access_token,id_token,referenceid,initdate) values(@sub,@name,@email,@access_token,@id_token,@referenceid,getdate())";
                        cmd.Parameters.Add(new SqlParameter("@sub", user.Id));
                        cmd.Parameters.Add(new SqlParameter("@name", user.Name));
                        cmd.Parameters.Add(new SqlParameter("@email", user.Email));
                        cmd.Parameters.Add(new SqlParameter("@access_token", user.AccessToken));
                        cmd.Parameters.Add(new SqlParameter("@refresh_token", user.RefreshToken));
                        cmd.Parameters.Add(new SqlParameter("@id_token", user.IdToken));
                        cmd.Parameters.Add(new SqlParameter("@referenceid", user.ReferenceId ?? string.Empty));

                        cmd.Connection.Open();
                        result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                result = -1;
                Console.WriteLine(ex.ToString());
            }
            return result;
        }

        public static int UpdateUser(OUser user)
        {
            var result = 0;
            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "update oUser set name =@name,email=@email,access_token=@access_token,id_token=@id_token,refresh_token=@refresh_token,referenceid=@referenceid where id = @sub";
                        cmd.Parameters.Add(new SqlParameter("@sub", user.Id));
                        cmd.Parameters.Add(new SqlParameter("@name", user.Name));
                        cmd.Parameters.Add(new SqlParameter("@email", user.Email));
                        cmd.Parameters.Add(new SqlParameter("@access_token", user.AccessToken));
                        cmd.Parameters.Add(new SqlParameter("@refresh_token", user.RefreshToken));
                        cmd.Parameters.Add(new SqlParameter("@id_token", user.IdToken));
                        cmd.Parameters.Add(new SqlParameter("@referenceid", user.ReferenceId));
                        cmd.Connection.Open();
                        result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                result = -1;
                Console.WriteLine(ex.ToString());
            }
            return result;
        }
    }


    public class OUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdToken { get; set; }
        public string ReferenceId { get; set; }
    }
}
