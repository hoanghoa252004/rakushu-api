# Video API - Quick Start Guide

## 🚀 Getting Started in 5 Minutes

### Step 1: Understand the API Structure
The Video API has 8 endpoints organized by functionality:

**Public Endpoints** (Anonymous access):
- `GET /api/videos/{id}` - Get video details
- `GET /api/videos/curated` - Browse curated videos

**User Endpoints** (Learner login required):
- `POST /api/videos` - Upload a video
- `PUT /api/videos/{id}` - Update your video
- `DELETE /api/videos/{id}` - Delete your Pending videos
- `GET /api/videos/me` - View your uploads

**Admin Endpoints** (Curator/Admin login required):
- `GET /api/videos` - View all system videos
- `PATCH /api/videos/{id}/status` - Change video status

### Step 2: Create Your First Video

```bash
curl -X POST http://localhost:5284/api/videos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
	"slug": "my-first-video",
	"title": "My First Video",
	"description": "This is my first video",
	"duration": "00:30:00",
	"contentCategoryId": "YOUR_CATEGORY_ID",
	"seriesId": null,
	"sortOrder": 1,
	"sourceType": "UploadedByLearner",
	"sourceUrl": null,
	"storageKeyOfVideo": "path/to/video.mp4"
  }'
```

**Important**: 
- `storageKeyOfVideo` must be obtained from the PresignedURL API first
- `sourceType` can be: `Curated` (curator only), `UploadedByLearner`, or `ImportedFromExternal`
- Response will include the new video ID

### Step 3: View Your Videos

```bash
# Get your own videos
curl "http://localhost:5284/api/videos/me?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer LEARNER_TOKEN"

# Get curated videos (public)
curl "http://localhost:5284/api/videos/curated?pageNumber=1&pageSize=10"

# Get all videos (admin/curator only)
curl "http://localhost:5284/api/videos?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer CURATOR_TOKEN"
```

### Step 4: Update Your Video

```bash
curl -X PUT http://localhost:5284/api/videos/{video-id} \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
	"title": "Updated Title",
	"description": "Updated description",
	"contentCategoryId": "YOUR_CATEGORY_ID",
	"seriesId": null,
	"sortOrder": 1
  }'
```

**Note**: You cannot change `sourceType` or `status` here. Use the status endpoint for status changes.

### Step 5: Change Video Status (Curator Only)

```bash
curl -X PATCH http://localhost:5284/api/videos/{video-id}/status \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer CURATOR_TOKEN" \
  -d '{
	"newStatus": "Processing"
  }'
```

Valid transitions:
- `Draft` → `Processing`
- `Pending` → `Processing` or `Rejected`
- `Processing` → `Reviewing`
- `Reviewing` → `Ready`

## 📊 Common Queries

### Search for Videos
```bash
# Search by title/slug
curl "http://localhost:5284/api/videos/curated?searchKey=japanese"
```

### Filter by Category
```bash
# Get videos in specific category
curl "http://localhost:5284/api/videos/curated?contentCategoryId=550e8400-e29b-41d4-a716-446655440000"
```

### Get Specific Video
```bash
curl "http://localhost:5284/api/videos/550e8400-e29b-41d4-a716-446655440001"
```

### Pagination
```bash
# Get second page with 20 items per page
curl "http://localhost:5284/api/videos/curated?pageNumber=2&pageSize=20"
```

## 🔐 Authorization Rules

### Who Can Do What?

| Action | Learner | Curator | Admin | Anonymous |
|--------|---------|---------|-------|-----------|
| Create video | Yes* | Yes** | No | No |
| Update video | Own only | Own only | No | No |
| Change status | No | Yes | No | No |
| Delete own Pending | Yes | No | No | No |
| Delete Curated Draft | No | Yes | No | No |
| View all videos | No | Yes | Yes | No |
| View own videos | Yes | No | No | No |
| View curated videos | No | No | No | Yes |
| View details | Yes | Yes | Yes | Yes |

*Learner: Can only use UploadedByLearner or ImportedFromExternal source
**Curator: Can use Curated source (only option for them)

## 🚨 Common Errors

### 400 Bad Request
**Cause**: Invalid input data
```json
{
  "errors": {
	"title": ["Title is required"]
  }
}
```
**Solution**: Check all required fields and formats

### 401 Unauthorized
**Cause**: Missing or invalid authentication token
**Solution**: Include valid Bearer token in Authorization header

### 403 Forbidden
**Cause**: User doesn't have permission
```json
{
  "code": "VIDEO.UNAUTHORIZED_VIDEO_SOURCE",
  "message": "Only curators can create videos with Curated source"
}
```
**Solution**: Check if your role has permission for this action

### 404 Not Found
**Cause**: Video doesn't exist
**Solution**: Verify the video ID is correct

### 409 Conflict
**Cause**: Invalid state transition or deletion rule
```json
{
  "code": "VIDEO.INVALID_STATUS_TRANSITION",
  "message": "Invalid status transition for video"
}
```
**Solution**: Can't transition to that status from current status

## 💾 Response Examples

### Get Video Details
```json
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

### List Videos
```json
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
  "totalCount": 150,
  "totalPages": 15,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

## 🎯 Typical User Workflows

### Learner Workflow: Upload & Manage Videos
1. Upload video using `POST /api/videos`
2. Get confirmation and video ID
3. View your uploads with `GET /api/videos/me`
4. Update video info with `PUT /api/videos/{id}`
5. Delete if status is still Pending with `DELETE /api/videos/{id}`
6. Wait for approval (status changes Pending → Processing → Reviewing → Ready)

### Curator Workflow: Review & Approve Videos
1. View pending videos with `GET /api/videos` (filter by status)
2. Review video details with `GET /api/videos/{id}`
3. Change status with `PATCH /api/videos/{id}/status`
4. Progress: Processing → Reviewing → Ready
5. Or reject: Pending → Rejected

### Admin Workflow: System Management
1. View all videos with `GET /api/videos`
2. Filter by user, category, or series
3. Export/analyze data
4. Monitor video distribution and usage

### Public User Workflow: Watch Videos
1. Browse curated videos with `GET /api/videos/curated`
2. Search by keyword
3. Filter by category or series
4. View video details with `GET /api/videos/{id}`
5. See media assets and creator info

## 📞 Troubleshooting

### "Only curators can create videos with Curated source"
- **Problem**: You're trying to create a Curated video as Learner
- **Solution**: Use UploadedByLearner or ImportedFromExternal source, or login as curator

### "Video not found"
- **Problem**: Video ID doesn't exist
- **Solution**: Copy ID from previous response, verify it's correct UUID format

### "Invalid status transition"
- **Problem**: Can't go from current status to requested status
- **Solution**: Check status transition rules above

### "Cannot delete this video"
- **Problem**: Video doesn't meet deletion criteria
- **Solution**: 
  - Learner: Video must be in Pending status
  - Curator: Video must be Curated and Draft status

## 🔗 Related Endpoints

These Video APIs work with other Rakushu APIs:

1. **PresignedURL API** - Get storageKey before creating video
2. **ContentCategory API** - Get category IDs for filtering
3. **Series API** - Get series IDs for filtering
4. **User API** - User account management
5. **Storage API** - Handle actual file uploads

## 📚 Full Documentation

For detailed API specifications, see:
- `IMPLEMENTATION_SUMMARY.md` - Complete specifications
- `VIDEO_API_QUICK_REFERENCE.md` - API reference
- Source code comments - Implementation details

## ✅ Checklist for First Integration

- [ ] Get authentication token
- [ ] Get storageKey from PresignedURL
- [ ] Get contentCategoryId from ContentCategory API
- [ ] Create first video with POST
- [ ] Verify video appears in results
- [ ] Update video with PUT
- [ ] Test pagination with pageNumber/pageSize
- [ ] Test filters (searchKey, contentCategoryId)
- [ ] Test status transitions (curator)
- [ ] Test deletion (if criteria met)

---

**Now You're Ready!** Start integrating the Video API into your application! 🎉
