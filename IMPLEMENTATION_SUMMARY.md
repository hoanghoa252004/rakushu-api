# Video API Implementation Summary

## Overview
Implemented 8 complete Video CRUD APIs with proper authorization, pagination, filtering, and validation as specified.

## API Endpoints

### 1. POST /api/videos - Create Video
- **Authorization**: Learner, LinguisticCurator
- **Description**: Creates a new video
- **Business Logic**:
  - Only Curator can create videos with `SourceType = Curated`
  - Learners can only upload with `SourceType = UploadedByLearner` or `ImportedFromExternal`
  - Automatic status assignment:
	- `Curated` source → `Processing` status
	- Other sources → `Pending` status
  - StorageKey is required (pre-signed URL from PresignedURL API)
  - CreatedBy is automatically set from current user context

**Request Body**:
```json
{
  "slug": "string",
  "title": "string",
  "description": "string (optional)",
  "duration": "TimeSpan",
  "contentCategoryId": "guid",
  "seriesId": "guid (optional)",
  "sortOrder": "int",
  "sourceType": "VideoSource enum",
  "sourceUrl": "string (optional)",
  "storageKeyOfVideo": "string"
}
```

**Response**: 201 Created with video ID

### 2. PUT /api/videos/{id:guid} - Update Video
- **Authorization**: Learner, LinguisticCurator
- **Description**: Updates only basic video info (NOT status)
- **Updatable Fields**: Title, Description, ContentCategoryId, SeriesId, SortOrder
- **Status**: Cannot be updated via this endpoint (use PATCH instead)

**Request Body**:
```json
{
  "title": "string",
  "description": "string (optional)",
  "contentCategoryId": "guid",
  "seriesId": "guid (optional)",
  "sortOrder": "int"
}
```

**Response**: 200 OK

### 3. GET /api/videos/curated - Get Curated Videos
- **Authorization**: Anonymous (AllowAnonymous)
- **Description**: Gets all videos with `SourceType = Curated`
- **Pagination**: pageNumber, pageSize (max 100)
- **Filters**:
  - `searchKey`: Searches title and slug
  - `contentCategoryId`: Filter by category
  - `seriesId`: Filter by series
- **Ordering**: By CreatedAt descending

**Query Parameters**:
```
GET /api/videos/curated?pageNumber=1&pageSize=10&searchKey=&contentCategoryId=&seriesId=
```

**Response**: 
```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### 4. GET /api/videos/me - Get My Videos
- **Authorization**: Learner
- **Description**: Gets videos created by current learner
- **Filter**: Only videos with `SourceType = UploadedByLearner` or `ImportedFromExternal`
- **Pagination**: pageNumber, pageSize (max 100)
- **Filters**:
  - `searchKey`: Searches title and slug
  - `contentCategoryId`: Filter by category
  - `seriesId`: Filter by series
- **Ordering**: By CreatedAt descending

**Query Parameters**:
```
GET /api/videos/me?pageNumber=1&pageSize=10&searchKey=&contentCategoryId=&seriesId=
```

**Response**: Same pagination structure as curated videos

### 5. GET /api/videos - Get All Videos
- **Authorization**: LinguisticCurator, SystemAdministrator
- **Description**: Gets all videos in system
- **Pagination**: pageNumber, pageSize (max 100)
- **Filters**:
  - `searchKey`: Searches title and slug
  - `contentCategoryId`: Filter by category
  - `seriesId`: Filter by series
  - `userId`: Filter by creator
- **Ordering**: By CreatedAt descending

**Query Parameters**:
```
GET /api/videos?pageNumber=1&pageSize=10&searchKey=&contentCategoryId=&seriesId=&userId=
```

**Response**: Same pagination structure as curated videos

### 6. GET /api/videos/{id:guid} - Get Video Details
- **Authorization**: Anonymous (AllowAnonymous)
- **Description**: Gets complete video information
- **Includes**:
  - Video basic info
  - Content category details
  - Series details (if applicable)
  - List of media assets
  - Creator information (FullName)

**Response**:
```json
{
  "id": "guid",
  "slug": "string",
  "title": "string",
  "description": "string",
  "duration": "TimeSpan",
  "contentCategoryId": "guid",
  "contentCategoryName": "string",
  "seriesId": "guid",
  "seriesName": "string",
  "sortOrder": "int",
  "sourceType": "VideoSource enum",
  "sourceUrl": "string",
  "status": "VideoStatus enum",
  "createdBy": "guid",
  "createdByFullName": "string",
  "createdAt": "ISO8601",
  "updatedAt": "ISO8601",
  "mediaAssets": [
	{
	  "id": "guid",
	  "storageKey": "string",
	  "contentType": "string",
	  "mediaType": "string (enum)"
	}
  ]
}
```

### 7. PATCH /api/videos/{id:guid}/status - Update Video Status
- **Authorization**: LinguisticCurator only
- **Description**: Updates video status with validation
- **Status Transitions** (from VideoStatusTransition):
  - Draft → Processing
  - Pending → Processing, Rejected
  - Processing → Reviewing
  - Reviewing → Ready
  - Rejected → (no transitions)
  - Archived → (no transitions)

**Request Body**:
```json
{
  "newStatus": "VideoStatus enum"
}
```

**Response**: 200 OK
**Error**: 409 Conflict if transition is invalid

### 8. DELETE /api/videos/{id:guid} - Delete Video
- **Authorization**: Learner, LinguisticCurator
- **Business Rules**:
  - **Learner**: Can delete their own videos only if status is `Pending`
  - **Curator**: Can delete `Curated` videos only if status is `Draft`
  - **Others**: Cannot delete

**Response**: 200 OK
**Error**: 409 Conflict if deletion rules are not met

## Domain Model Changes

### Video Entity Enhancements
```csharp
// NEW METHOD: UpdateBasicInfo()
public Result UpdateBasicInfo(
	string title,
	string? description,
	ContentCategoryId contentCategoryId,
	SeriesId? seriesId,
	int sortOrder,
	DateTimeOffset updatedAt)
```

```csharp
// NEW METHOD: UpdateStatus()
public Result UpdateStatus(VideoStatus newStatus)
```

### VideoErrors Enhancements
Added new error types:
- `InvalidVideoSource`
- `InvalidVideoStatus`
- `InvalidStatusTransition`
- `UnauthorizedVideoSourceForCurator`
- `CannotDeleteVideo`
- `CannotUpdateVideoStatus`

## Application Layer Structure

### Commands
- `CreateVideoCommand` - Create new video
- `UpdateVideoCommand` - Update basic video info
- `UpdateVideoStatusCommand` - Update video status
- `DeleteVideoCommand` - Delete video

### Queries
- `GetVideoByIdQuery` - Get video details
- `GetCuratedVideosQuery` - Get curated videos with pagination
- `GetMyVideosQuery` - Get learner's videos with pagination
- `GetAllVideosQuery` - Get all videos (admin/curator) with pagination

### Validators
All commands/queries have FluentValidation validators for:
- Required field validation
- Length validation
- Enum validation
- Pagination parameter validation

### DTOs
- `VideoListDto` - For list views (pagination endpoints)
- `VideoDetailDto` - For detailed view with related entities
- `MediaAssetDto` - Media asset information

## API Request/Response Validation

### Input Validation
- **Slug & Title**: Not empty, max 200 characters
- **Description**: Max 1000 characters
- **Duration**: Must be greater than zero
- **ContentCategoryId**: Required and must be valid GUID
- **SortOrder**: Must be greater than 0
- **SourceType**: Must be valid VideoSource enum
- **StorageKeyOfVideo**: Required for create
- **Pagination**: PageNumber > 0, PageSize 1-100

### Authorization Handling
- Used `RequireAuthorization` with role policies
- `AllowAnonymous` for public endpoints
- Current user context injected automatically from `ICurrentUserContext`
- User ID and role validation in handlers

## Status Transition Rules
```
Draft → Processing
Pending → Processing, Rejected
Processing → Reviewing
Reviewing → Ready
Ready → (no forward transitions, must be Archived separately)
Rejected → (terminal state)
Archived → (terminal state)
```

## Key Features

1. **Automatic Status Assignment**: Based on VideoSource
2. **Role-Based Access Control**: Different operations for Learner/Curator/Admin
3. **Status Validation**: Uses VideoStatusTransition for valid transitions
4. **Pagination Support**: Implements PaginatedList for large datasets
5. **Filtering**: SearchKey, CategoryId, SeriesId, UserId filters
6. **Comprehensive DTOs**: Specialized for different use cases
7. **Full Validation**: FluentValidation on all inputs
8. **Error Handling**: Specific error codes for each failure scenario

## Testing Recommendations

1. **Authorization Tests**: Verify role-based access
2. **Status Transition Tests**: Ensure only valid transitions
3. **Pagination Tests**: Verify filtering and sorting
4. **Deletion Tests**: Verify deletion rules for Learner and Curator
5. **Video Creation Tests**: Verify automatic status assignment
6. **Update Tests**: Verify non-status fields can be updated
7. **DTOs Tests**: Verify correct data mapping from entities
