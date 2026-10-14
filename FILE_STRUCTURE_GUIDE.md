# Video API - File Structure & Location Guide

## Project Structure Overview

```
Rakushu.Api
├── Endpoints
│   └── Learning
│       └── Video
│           ├── VideoEndpointGroupExtension.cs ✅ Route group configuration
│           ├── CreateVideo/
│           │   └── CreateVideo.cs ✅
│           ├── UpdateVideo/
│           │   └── UpdateVideo.cs ✅
│           ├── GetCuratedVideos/
│           │   └── GetCuratedVideos.cs ✅
│           ├── GetMyVideos/
│           │   └── GetMyVideos.cs ✅
│           ├── GetAllVideos/
│           │   └── GetAllVideos.cs ✅
│           ├── GetVideoById/
│           │   └── GetVideoById.cs ✅ (UPDATED)
│           ├── UpdateVideoStatus/
│           │   └── UpdateVideoStatus.cs ✅ (NEW)
│           └── DeleteVideo/
│               └── DeleteVideo.cs ✅ (UPDATED)

Rakushu.Application
└── Usecases
	└── Learning
		└── Video
			├── VideoListDto.cs ✅ (NEW)
			├── VideoDetailDto.cs ✅ (NEW - combines DetailDto + MediaAssetDto)
			├── VideoDto.cs (Original - kept for compatibility)
			├── CreateVideo/
			│   ├── CreateVideoCommand.cs ✅ (UPDATED)
			│   ├── CreateVideoHandler.cs ✅ (UPDATED)
			│   └── CreateVideoValidator.cs ✅ (NEW)
			├── UpdateVideo/
			│   ├── UpdateVideoCommand.cs ✅ (UPDATED)
			│   ├── UpdateVideoHandler.cs ✅ (UPDATED)
			│   └── UpdateVideoValidator.cs ✅ (NEW)
			├── GetCuratedVideos/
			│   ├── GetCuratedVideosQuery.cs ✅ (NEW)
			│   ├── GetCuratedVideosHandler.cs ✅ (NEW)
			│   └── GetCuratedVideosValidator.cs ✅ (NEW)
			├── GetMyVideos/
			│   ├── GetMyVideosQuery.cs ✅ (NEW)
			│   ├── GetMyVideosHandler.cs ✅ (NEW)
			│   └── GetMyVideosValidator.cs ✅ (NEW)
			├── GetAllVideos/
			│   ├── GetAllVideosQuery.cs ✅ (NEW)
			│   ├── GetAllVideosHandler.cs ✅ (NEW)
			│   └── GetAllVideosValidator.cs ✅ (NEW)
			├── GetVideoById/
			│   ├── GetVideoByIdQuery.cs (Existing)
			│   └── GetVideoByIdHandler.cs (Existing)
			├── UpdateVideoStatus/
			│   ├── UpdateVideoStatusCommand.cs ✅ (NEW)
			│   ├── UpdateVideoStatusHandler.cs ✅ (NEW)
			│   └── UpdateVideoStatusValidator.cs ✅ (NEW)
			└── DeleteVideo/
				├── DeleteVideoCommand.cs (Existing)
				├── DeleteVideoHandler.cs ✅ (UPDATED)
				└── DeleteVideoValidator.cs ✅ (NEW)

Rakushu.Domain
└── Entities
	└── Video
		├── Video.cs ✅ (UPDATED - Added UpdateBasicInfo(), UpdateStatus())
		├── VideoErrors.cs ✅ (UPDATED - Added 6 new error types)
		├── VideoStatus.cs (Existing - Enum)
		├── VideoSource.cs (Existing - Enum)
		├── VideoStatusTransition.cs (Existing - Status validation)
		├── VideoId.cs (Existing)
		└── IVideoRepository.cs (Existing)
```

## File Changes Summary

### CREATED Files (19 new)
```
API Layer (4):
✅ GetCuratedVideos.cs
✅ GetMyVideos.cs
✅ GetAllVideos.cs
✅ UpdateVideoStatus.cs

Application Layer - DTOs (2):
✅ VideoListDto.cs
✅ VideoDetailDto.cs

Application Layer - GetCuratedVideos (3):
✅ GetCuratedVideosQuery.cs
✅ GetCuratedVideosHandler.cs
✅ GetCuratedVideosValidator.cs

Application Layer - GetMyVideos (3):
✅ GetMyVideosQuery.cs
✅ GetMyVideosHandler.cs
✅ GetMyVideosValidator.cs

Application Layer - GetAllVideos (3):
✅ GetAllVideosQuery.cs
✅ GetAllVideosHandler.cs
✅ GetAllVideosValidator.cs

Application Layer - UpdateVideoStatus (3):
✅ UpdateVideoStatusCommand.cs
✅ UpdateVideoStatusHandler.cs
✅ UpdateVideoStatusValidator.cs

Application Layer - Validators (1):
✅ DeleteVideoValidator.cs
```

### UPDATED Files (7)
```
API Layer (1):
✅ VideoEndpointGroupExtension.cs - Changed route from /api/admin/videos to /api/videos

API Layer - CreateVideo (1):
✅ CreateVideo.cs - Added auth, validators, documentation

API Layer - UpdateVideo (1):
✅ UpdateVideo.cs - Updated to use new DTO, added auth, documentation

API Layer - GetVideoById (1):
✅ GetVideoById.cs - Added auth, improved documentation

API Layer - DeleteVideo (1):
✅ DeleteVideo.cs - Added auth and business rules documentation

Application Layer - Commands (2):
✅ CreateVideoCommand.cs - Restructured parameters, added StorageKeyOfVideo
✅ UpdateVideoCommand.cs - Simplified to only basic fields

Application Layer - Handlers (2):
✅ CreateVideoHandler.cs - Added authorization logic, status assignment
✅ UpdateVideoHandler.cs - Updated to use UpdateBasicInfo()
✅ DeleteVideoHandler.cs - Added role-based deletion rules

Domain Layer (2):
✅ Video.cs - Added UpdateBasicInfo() and UpdateStatus() methods
✅ VideoErrors.cs - Added 6 new error types
```

## Key Implementation Details

### Routes Summary
```
POST   /api/videos                    - Create
PUT    /api/videos/{id:guid}          - Update
PATCH  /api/videos/{id:guid}/status   - Update Status
DELETE /api/videos/{id:guid}          - Delete
GET    /api/videos                    - Get all (admin/curator)
GET    /api/videos/{id:guid}          - Get by ID
GET    /api/videos/me                 - Get my videos (learner)
GET    /api/videos/curated            - Get curated (any)
```

### Authorization Levels
```
Anonymous (2):
  - GET /api/videos/curated
  - GET /api/videos/{id}

Learner (3):
  - POST /api/videos (with restrictions on SourceType)
  - PUT /api/videos/{id}
  - DELETE /api/videos/{id} (Pending status only, own videos)
  - GET /api/videos/me

Curator (5):
  - POST /api/videos (can use Curated source)
  - PUT /api/videos/{id}
  - PATCH /api/videos/{id}/status
  - DELETE /api/videos/{id} (Curated + Draft status)
  - GET /api/videos (all videos)

Admin (1):
  - GET /api/videos (all videos)
```

### Database Transactions
- All write operations use `IUnitOfWork.ExecuteAsync()` for transaction management
- All mutations are wrapped in a transaction

### Validation Framework
- Uses FluentValidation for all input validation
- Validators auto-discovered and executed by FluentValidationMiddleware
- Both command/query and API endpoint DTO validation

### Error Handling
- Result<T> pattern for domain operations
- Error codes for API responses (e.g., "VIDEO.NOT_FOUND")
- Proper HTTP status codes (201, 200, 400, 403, 404, 409, 500)

### DTO Mapping
- VideoListDto: For paginated list responses
- VideoDetailDto: For single video detail response
- MediaAssetDto: Embedded in VideoDetailDto
- Request DTOs: Separate for POST/PUT/PATCH operations

### Dependency Injection
- IVideoRepository - Video data access
- ICurrentUserContext - Current user identification
- IUnitOfWork - Transaction management
- ISystemClock - Consistent time handling
- ISender - MediatR request dispatcher

## Testing Recommendations

### Unit Tests
- Validators for all commands/queries
- Status transition logic
- Authorization checks
- Error scenarios

### Integration Tests
- End-to-end API flows
- Database transactions
- Pagination logic
- Filtering combinations

### Security Tests
- Role-based access control
- Ownership validation
- Status-based access control
- Enum value validation

## Performance Considerations

1. **Pagination**: Implemented with skip/take to handle large datasets
2. **Filtering**: Done in-memory after database retrieval (consider moving to database for large datasets)
3. **Sorting**: By CreatedAt descending for reverse chronological order
4. **Relations**: Navigation properties loaded automatically via ORM

## Future Enhancements

1. **Database-level Filtering**: Move filtering to database queries instead of LINQ-to-Object
2. **Caching**: Add caching for frequently accessed videos
3. **Search**: Enhanced full-text search capabilities
4. **Batch Operations**: Bulk video operations
5. **Soft Delete**: Instead of hard delete
6. **Audit Trail**: Track who modified videos and when
7. **Version Control**: Multiple versions of videos
8. **Comments**: Add user comments/reviews on videos
