using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Series;

public interface ISeriesRepository
{
	Task<Series?> GetByIdAsync(SeriesId id, CancellationToken cancellationToken = default);
	Task<Series?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
	Task<IEnumerable<Series>> GetAllAsync(CancellationToken cancellationToken = default);
	void Add(Series series);
	void Delete(Series series);
}
