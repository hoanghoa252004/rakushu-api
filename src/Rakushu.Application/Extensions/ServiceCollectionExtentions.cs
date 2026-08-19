using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Application.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Extensions;

public static class ApplicationServiceCollectionExtentions
{
	public static IServiceCollection AddRakushuApplication(this IServiceCollection services)
	{
		var assembly = typeof(ApplicationServiceCollectionExtentions).Assembly;

		services.AddMediatR(cfg =>
		{
			cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODE0NDAwMDAwIiwiaWF0IjoiMTc4Mjg4MjExOSIsImFjY291bnRfaWQiOiIwMTliNTNiOThhNmU3ZjA3OWU2MmVmOGRjYjQ1YmY0NCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2Q5dmt5Z3B3N2pobXpxbWh4MTJrOW45Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.Rbah_Q349_QRF_b1eJYrkI2t3c0ldLytvzk_bhuJRkera4Q_Xwm8r2lZ6vqj2UwPPsCH__u95pwts5lmDkzTLLYBIy4ER9MpsF61o6gVof0LtxAqWOZ4faufZQLu7kUKfDe_wQl59bv43rk2pNFxC6DPaa_zUxS1MRftBUZAksywntkOOWo1xOsMQq5GDcRbB2y6Nq4YbOXneyXJW-QIdzlAQDbrS7pj0QpAo1rDcCjabukRiLwlO9sxo2ElaUu0dKNX8edxBafIsrCNFE9S5m1QzW6DwNuJJ_oFh5xY1FEJmARMBIjcyY8TGYCk60S_qSfVsaAfEhNbLA8Ccr-mHQ";

			cfg.RegisterServicesFromAssembly(assembly);

			cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
			cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

		});

		services.AddValidatorsFromAssembly(assembly,
			includeInternalTypes: true);

		return services;
	}
}
