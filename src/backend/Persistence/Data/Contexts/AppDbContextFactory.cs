using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Contexts
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1. Lấy đường dẫn thư mục hiện tại (nơi em đang đứng gõ lệnh)
            var currentDirectory = Directory.GetCurrentDirectory();

            // 2. Tự động dò tìm thư mục Api dựa trên cấu trúc project của em
            // Thầy dùng Path.GetFullPath để làm sạch đường dẫn, tránh lỗi dấu xuyệc
            var apiPath = Path.GetFullPath(Path.Combine(currentDirectory, "src/backend/Api"));

            // Kiểm tra xem thư mục có tồn tại không để báo lỗi rõ ràng hơn
            if (!Directory.Exists(apiPath))
            {
                // Nếu em đang đứng ở src/backend/Persistence thì dùng đường dẫn này
                apiPath = Path.GetFullPath(Path.Combine(currentDirectory, "../Api"));
            }

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options, new DesignTimeUserContext());
        }
        //public AppDbContext CreateDbContext(string[] args)
        //{
        //    // Sử dụng đường dẫn tuyệt đối thẳng tới thư mục Api của em
        //    // Lưu ý: Kiểm tra kỹ xem chữ 'Api' có viết hoa hay không (Windows không phân biệt nhưng tốt nhất nên viết đúng)
        //    var apiPath = @"../greenginger";

        //    // Kiểm tra nhanh xem đường dẫn có đúng không
        //    if (!File.Exists(Path.Combine(apiPath, "appsettings.json")))
        //    {
        //        throw new Exception($"Không tìm thấy file appsettings.json tại: {apiPath}");
        //    }

        //    IConfigurationRoot configuration = new ConfigurationBuilder()
        //        .SetBasePath(apiPath)
        //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //        .Build();

        //    var connectionString = configuration.GetConnectionString("DefaultConnection");

        //    // Nếu connectionString null, hãy kiểm tra lại key "DefaultConnection" trong file json
        //    if (string.IsNullOrEmpty(connectionString))
        //    {
        //        throw new Exception("ConnectionString 'DefaultConnection' bị trống hoặc không tìm thấy.");
        //    }

        //    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        //    optionsBuilder.UseSqlServer(connectionString);

        //    return new AppDbContext(optionsBuilder.Options, new DesignTimeUserContext());
        //}
    }

    public class DesignTimeUserContext : IUserContext
    {
        public string UserName => "Migration";
        public string? UserId => null;
        public bool IsAuthenticated => false;
    }
}
