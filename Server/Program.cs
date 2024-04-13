using Server.Middleware;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Максимальное количество параллельно обрабатываемых запросов: ");
            var taskLimit = int.Parse(Console.ReadLine());

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.UseMiddleware<ParallelTaskLimitMiddleware>(taskLimit);

            app.Run();
        }
    }
}
