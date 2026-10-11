# Video API Quick Reference Guide

## Base URL
```
/api/videos
```

## All Endpoints Summary

| # | Method | Endpoint | Auth | Purpose |
|----|--------|----------|------|---------|
| 1  | POST   | /        | Learner, Curator | Create video |
| 2  | PUT    | /{id}    | Learner, Curator | Update video info |
| 3  | GET    | /curated | Anonymous | Get curated videos (paginated) |
| 4  | GET    | /me      | Learner | Get my videos (paginated) |
| 5  | GET    | /        | Curator, Admin | Get all videos (paginated) |
| 6  | GET    | /{id}    | Anonymous | Get video details |
| 7  | PATCH  | /{id}/status | Curator | Update video status |
| 8  | DELETE | /{id}    | Learner, Curator | Delete video |

## Authorization Summary

```
Endpoint                          Learner    Curator    Admin      Anonymous
POST /api/videos                   ✓         ✓          ✗          ✗
PUT /api/videos/{id}               ✓         ✓          ✗          ✗
GET /api/videos/curated           ✗         ✗          ✗          ✓
GET /api/videos/me                ✓         ✗          ✗          ✗
GET /api/videos                   ✗         ✓          ✓          ✗
GET /api/videos/{id}              ✓         ✓          ✓          ✓
PATCH /api/videos/{id}/status     ✗         ✓          ✗          ✗
DELETE /api/videos/{id}           ✓ (rules) ✓ (rules)  ✗          ✗
```

## Request/Response Examples

### 1. Create Video
```bash
POST /api/videos
Content-Type: application/json
Authorization: Bearer <token>

{
  "slug": "intro-to-japanese",
  "title": "Introduction to Japanese",
  "description": "Basic Japanese lessons",
  "duration": "00:45:30",
  "contentCategoryId": "550e8400-e29b-41d4-a716-446655440000",
  "seriesId": null,
  "sortOrder": 1,
  "sourceType": "Curated",
  "sourceUrl": null,
  "storageKeyOfVideo": "videos/japanese/intro.mp4"
}

Response: 201 Created
Location: /api/videos/550e8400-e29b-41d4-a716-446655440001
```

### 2. Update Video
```bash
PUT /api/videos/550e8400-e29b-41d4-a716-446655440001
Content-Type: application/json
Authorization: Bearer <token>

{
  "title": "Introduction to Japanese - Updated",
  "description": "Complete Japanese lessons for beginners",
  "contentCategoryId": "550e8400-e29b-41d4-a716-446655440000",
  "seriesId": null,
  "sortOrder": 2
}

Response: 200 OK
```

### 3. Get Curated Videos
```bash
GET /api/videos/curated?pageNumber=1&pageSize=10&searchKey=japanese&contentCategoryId=550e8400-e29b-41d4-a716-446655440000

Response: 200 OK
{
  "items": [
	{
	  "id": "550e8400-e29b-41d4-a716-446655440001",
	  "slug": "intro-to-japanese",
	  "title": "Introduction to Japanese",
	  "description": "Basic Japanese lessons",
	  "duration": "00:45:30",
	  "contentCategoryId": "550e8400-e29b-41d4-a716-446655440000",
	  "seriesId": null,
	  "sortOrder": 1,
	  "sourceType": "Curated",
	  "status": "Processing",
	  "createdBy": "550e8400-e29b-41d4-a716-446655440099",
	  "createdAt": "2024-01-15T10:30:00Z",
	  "updatedAt": "2024-01-15T10:30:00Z"
	}
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

### 4. Get My Videos (Learner)
```bash
GET /api/videos/me?pageNumber=1&pageSize=10
Authorization: Bearer <learner-token>

Response: 200 OK
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 5,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

### 5. Get All Videos (Admin/Curator)
```bash
GET /api/videos?pageNumber=1&pageSize=20&userId=550e8400-e29b-41d4-a716-446655440099
Authorization: Bearer <curator-token>

Response: 200 OK
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### 6. Get Video Details
```bash
GET /api/videos/550e8400-e29b-41d4-a716-446655440001

Response: 200 OK
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "slug": "intro-to-japanese",
  "title": "Introduction to Japanese",
  "description": "Basic Japanese lessons",
  "duration": "00:45:30",
  "contentCategoryId": "550e8400-e29b-41d4-a716-446655440000",
  "contentCategoryName": "Language Learning",
  "seriesId": null,
  "seriesName": null,
  "sortOrder": 1,
  "sourceType": "Curated",
  "sourceUrl": null,
  "status": "Processing",
  "createdBy": "550e8400-e29b-41d4-a716-446655440099",
  "createdByFullName": "John Doe",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z",
  "mediaAssets": [
	{
	  "id": "550e8400-e29b-41d4-a716-446655440050",
	  "storageKey": "videos/japanese/intro.mp4",
	  "contentType": "video/mp4",
	  "mediaType": "Video"
	}
  ]
}
```

### 7. Update Video Status
```bash
PATCH /api/videos/550e8400-e29b-41d4-a716-446655440001/status
Content-Type: application/json
Authorization: Bearer <curator-token>

{
  "newStatus": "Reviewing"
}

Response: 200 OK
```

### 8. Delete Video
```bash
DELETE /api/videos/550e8400-e29b-41d4-a716-446655440001
Authorization: Bearer <learner-token>

Response: 200 OK

# Errors:
# 403 Forbidden - If user doesn't own the video or status is not Pending (learner)
# 403 Forbidden - If video is not Curated source or status is not Draft (curator)
# 404 Not Found - If video doesn't exist
```

## Status Transition Flow

```
Draft
  ↓
Processing → Reviewing → Ready
  ↑
Pending → Rejected (terminal)

Archived (terminal)
```

## Error Responses

### 400 Bad Request
```json
{
  "errors": [
	{
	  "field": "title",
	  "message": "Title is required"
	}
  ],
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400
}
```

### 401 Unauthorized
```json
{
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication failed"
}
```

### 403 Forbidden
```json
{
  "title": "Forbidden",
  "status": 403,
  "detail": "Only curators can create videos with Curated source"
}
```

### 404 Not Found
```json
{
  "code": "VIDEO.NOT_FOUND",
  "message": "The video was not found.",
  "type": "NotFound",
  "status": 404
}
```

### 409 Conflict
```json
{
  "code": "VIDEO.INVALID_STATUS_TRANSITION",
  "message": "Invalid status transition for video.",
  "type": "Conflict",
  "status": 409
}
```

## Query Parameters

### Pagination
- `pageNumber` - Page number (default: 1, min: 1)
- `pageSize` - Items per page (default: 10, max: 100)

### Filters
- `searchKey` - Search in title and slug
- `contentCategoryId` - Filter by category UUID
- `seriesId` - Filter by series UUID
- `userId` - Filter by creator UUID (admin/curator only)

## Enum Values

### VideoSource
- `Curated` = 1
- `UploadedByLearner` = 2
- `ImportedFromExternal` = 3

### VideoStatus
- `Draft` = 1
- `Pending` = 2
- `Rejected` = 3
- `Processing` = 4
- `Reviewing` = 5
- `Ready` = 6
- `Archived` = 7

### MediaAssetType
- `Video` = 1
- `Audio` = 2
- `Thumbnail` = 3
- `Preview` = 4

## Important Notes

1. **SeriesId is Optional**: Use `null` for videos not in a series
2. **Automatic Status**: Status is automatically set when creating videos:
   - Curated source → Processing
   - Learner sources → Pending
3. **StorageKey Required**: Must be obtained from PresignedURL API first
4. **Update vs Create**: 
   - Create validates SourceType (Curator-only for Curated)
   - Update cannot change SourceType or Status
5. **Deletion Rules**:
   - Learner: Own videos, Pending status only
   - Curator: Curated videos, Draft status only
6. **Status Update**:
   - Only Curator can update status
   - Must follow valid transitions (see status flow)
7. **Pagination**: Always returns PaginatedList with metadata
8. **Timestamps**: All timestamps in ISO 8601 UTC format
