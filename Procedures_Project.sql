--GET--

CREATE PROCEDURE [dbo].[GetProfileById]
	@Id NVARCHAR(50)
AS
BEGIN
	SELECT p.Email, 
		   p.FirstName,
		   p.LastName,
		   i.Url as Url,
		   p.BirthDate
	FROM Profiles p
	LEFT JOIN Images i
	on p.ImageId = i.Id
	WHERE p.Email = @Id;
END

CREATE PROCEDURE [dbo].[GetGalleryById]
	@Id NVARCHAR(50)
AS
BEGIN
	SELECT g.Id,
		   g.Name,
		   g.UserId
	FROM Gallery g
	WHERE g.Id = @Id
END

CREATE PROCEDURE [dbo].[GetGalleryByUserId]
	@UserId NVARCHAR(50)
AS
BEGIN
	SELECT g.Id,
		   g.Name,
		   g.UserId
	FROM Gallery g
	WHERE g.UserId = @UserId
END

CREATE PROCEDURE [dbo].[GetImagesByGalleryId]
	@Id INT
AS
BEGIN
	SELECT i.Id,
		   i.Url,
		   i.GalleryId
	FROM Images i
	WHERE i.GalleryId = @Id
END

CREATE PROCEDURE [dbo].[GetImagesById]
	@Id INT
AS
BEGIN
	SELECT i.Id,
		   i.Url,
		   i.GalleryId
	FROM Images i
	WHERE i.Id = @Id
END

CREATE PROCEDURE [dbo].[GetFriendsByUserId]
	@UserId1 NVARCHAR(50)
AS
BEGIN
	SELECT f.Id,
		   f.UserId1,
		   f.UserId2
	FROM Friends f
	WHERE f.UserId1 = @UserId1
END

CREATE PROCEDURE [dbo].[GetAllProfiles]
	@UserId NVARCHAR(50)
AS
BEGIN
	SELECT p.Email, 
		   p.FirstName,
		   p.LastName,
		   i.Url as Url,
		   p.BirthDate
	FROM Profiles p
	LEFT JOIN Images i
	on p.ImageId = i.Id
	WHERE p.Email != @UserId;
END

CREATE PROCEDURE [dbo].[GetFriendsById]
	@Id INT
AS
BEGIN
	SELECT f.Id,
		   f.UserId1,
		   f.UserId2
	FROM Friends f
	WHERE f.Id = @Id
END

CREATE PROCEDURE [dbo].[GetProfilesCount]
AS
BEGIN
	SELECT COUNT(*)
	FROM Profiles
END

CREATE PROCEDURE [dbo].[GetLikesCountByPostId]
	@PostId INT
AS
BEGIN
	SELECT COUNT(*) as Count
	FROM Likes l
	WHERE l.PostId = @PostId
END

CREATE PROCEDURE [dbo].[GetLikesByPostId]
	@PostId INT
AS
BEGIN
	SELECT l.Id,
		   l.UserId,
		   l.PostId
	FROM Likes l
	WHERE l.PostId = @PostId
END

CREATE PROCEDURE [dbo].[GetLikesById]
	@Id INT
AS
BEGIN
	SELECT l.Id,
		   l.UserId,
		   l.PostId
	FROM Likes l
	WHERE l.Id = @Id
END

CREATE PROCEDURE [dbo].[GetCommentsByPostId]
	@PostId NVARCHAR(50)
AS
BEGIN
	SELECT c.Id,
		   c.UserId,
		   c.PostId,
		   i.Url as Url,
		   c.Text
	FROM Comments c
	LEFT JOIN Images i
	ON c.ImageId = i.Id
	WHERE c.PostId = @PostId
END

CREATE PROCEDURE [dbo].[GetCommentById]
	@Id INT
AS
BEGIN
	SELECT c.Id,
		   c.UserId,
		   c.PostId,
		   i.Url as Url,
		   c.Text
	FROM Comments c
	LEFT JOIN Images i
	ON c.ImageId = i.Id
	WHERE c.Id = @Id
END

CREATE PROCEDURE [dbo].[GetPostsByUserId]
	@UserId NVARCHAR(50)
AS
BEGIN
	SELECT p.Id,
		   p.Text,
		   i.Url as Url,
		   p.UserId,
		   p.CreateDate
	FROM Posts p
	LEFT JOIN Images i
	ON p.ImageId = i.Id
	WHERE p.UserId = @UserId
END

CREATE PROCEDURE [dbo].[GetPostsByFriends]
	@UserId NVARCHAR(50)
AS
BEGIN
	SELECT p.Id,
		   p.Text,
		   i.Url as Url,
		   p.UserId,
		   p.CreateDate
	FROM Posts p
	LEFT JOIN Images i
	ON p.ImageId = i.Id
	LEFT JOIN Friends f
	ON f.UserId1 = @UserId
	WHERE p.UserId = f.UserId2
END

CREATE PROCEDURE [dbo].[GetPostById]
	@Id INT
AS
BEGIN
	SELECT p.Id,
		   p.Text,
		   i.Url as Url,
		   p.UserId,
		   p.CreateDate
	FROM Posts p
	LEFT JOIN Images i
	ON p.ImageId = i.Id
	WHERE p.Id = @Id
END



--Delete--



CREATE PROCEDURE [dbo].[DeleteFriends]
	@Id INT
AS
BEGIN
	DELETE f
	FROM Friends f
	WHERE f.Id = @Id;
END

CREATE PROCEDURE [dbo].[DeleteImages]
	@Id INT
AS
BEGIN
	UPDATE Comments
	SET ImageId = NULL
	FROM Comments c
	WHERE c.ImageId = @Id

	UPDATE Posts
	SET ImageId = NULL
	FROM Posts p
	WHERE p.ImageId = @Id;

	UPDATE Profiles
	SET ImageId = NULL
	FROM Profiles p
	WHERE p.ImageId = @Id;
	
	DELETE i
	FROM Images i
	WHERE i.Id = @Id;
END

CREATE PROCEDURE [dbo].[DeleteLikes]
	@Id INT
AS
BEGIN
	DELETE l
	FROM Likes l
	WHERE l.Id = @Id;
END

CREATE PROCEDURE [dbo].[DeleteComments]
	@Id INT
AS
BEGIN
	DELETE c
	FROM Comments c
	WHERE c.Id = @Id;
END

CREATE PROCEDURE [dbo].[DeletePosts]
	@Id INT
AS
BEGIN
	DELETE c
	FROM Comments c
	WHERE c.PostId = @Id;

	DELETE l
	FROM Likes l
	WHERE l.PostId = @Id;

	DELETE p
	FROM Posts p
	WHERE p.Id = @Id;
END

CREATE PROCEDURE [dbo].[DeleteGallery]
	@Id INT
AS
BEGIN
	DELETE i
	FROM Images i
	WHERE i.GalleryId = @Id;

	DELETE g
	FROM Gallery g
	WHERE g.Id = @Id;
END

CREATE PROCEDURE [dbo].[DeleteProfiles]
	@Id NVARCHAR(50)
AS
BEGIN
	DELETE c
	FROM Comments c
	WHERE c.UserId = @Id;

	DELETE f
	FROM Friends f
	WHERE f.UserId1 = @Id OR f.UserId2 = @Id;

	DELETE g
	FROM Gallery g
	WHERE g.UserId = @Id;

	DELETE l
	FROM Likes l
	WHERE l.UserId = @Id;

	DELETE p
	FROM Post p
	WHERE p.UserId = @Id;

	DELETE p
	FROM Profiles p
	WHERE P.Email = @Id;
END

--Insert--

CREATE PROCEDURE [dbo].[InsertImage]
	@Url NVARCHAR(MAX),
	@GalleryId INT
AS
BEGIN
	INSERT INTO Images(Url, GalleryId)
	VALUES (@Url, @GalleryId)
END

CREATE PROCEDURE [dbo].[InsertGallery]
	@Name NVARCHAR(50),
	@UserId NVARCHAR(50)
AS
BEGIN
	INSERT INTO Gallery(Name, UserId)
	VALUES (@Name, @UserId)
END

CREATE PROCEDURE [dbo].[InsertLike]
	@PostId INT,
	@UserId NVARCHAR(50)
AS
BEGIN
	INSERT INTO Likes(PostId, UserId)
	VALUES (@PostId, @UserId)
END

CREATE PROCEDURE [dbo].[InsertFriend]
	@UserId1 NVARCHAR(50),
	@UserId2 NVARCHAR(50)
AS
BEGIN
	INSERT INTO Friends(UserId1, UserId2)
	VALUES (@UserId1, @UserId2)
END

CREATE PROCEDURE [dbo].[InsertComment]
	@UserId nvarchar(50),
	@PostId INT,
	@ImageUrl nvarchar(max),
	@Text nvarchar(max)
AS
BEGIN
	IF(@ImageUrl IS NOT NULL)
	BEGIN
		IF EXISTS (SELECT g.Name FROM Gallery g WHERE g.Name = 'Comments')
		BEGIN 
			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Comments';
		END
		ELSE
		BEGIN
			INSERT INTO Gallery(Name, UserId)
			VALUES ('Comments', @UserId)

			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Comments';
		END
	END

	INSERT INTO Comments(UserId, PostId, ImageId, Text)
	SELECT @UserId, @PostId, MAX(i.Id), @Text FROM Images i
END

CREATE PROCEDURE [dbo].[InsertPost]
	@UserId nvarchar(50),
	@CreateDate datetime,
	@ImageUrl nvarchar(max),
	@Text nvarchar(max)
AS
BEGIN
	IF (@ImageUrl IS NOT NULL)
	BEGIN
		IF EXISTS (SELECT g.Name FROM Gallery g WHERE g.Name = 'Posts')
		BEGIN 
			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Posts';
		END
		ELSE
		BEGIN
			INSERT INTO Gallery(Name, UserId)
			VALUES ('Posts', @UserId)

			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Posts';
		END
	END

	INSERT INTO Posts(UserId, CreateDate, ImageId, Text)
	SELECT @UserId, @CreateDate, MAX(i.Id), @Text FROM Images i
END

CREATE PROCEDURE [dbo].[InsertProfile]
	@UserId nvarchar(50),
	@FirstName nvarchar(50),
	@LastName nvarchar(max),
	@BirthDate datetime,
	@ImageUrl nvarchar(max)
AS
BEGIN
	IF (@ImageUrl IS NOT NULL)
	BEGIN
		IF EXISTS (SELECT g.Name FROM Gallery g WHERE g.Name = 'Profiles')
		BEGIN 
			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Profiles';
		END
		ELSE
		BEGIN
			INSERT INTO Gallery(Name, UserId)
			VALUES ('Profiles', @UserId)

			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Profiles';
		END
	END

	INSERT INTO Profiles(Email, FirstName, LastName, ImageId,BirthDate)
	SELECT @UserId, @FirstName, @LastName, MAX(i.Id), @BirthDate FROM Images i
END

--Update--

CREATE PROCEDURE [dbo].[UpdateImage]
	@Id INT,
	@Url NVARCHAR(MAX),
	@GalleryId INT
AS
BEGIN
	UPDATE Images
	SET Url = @Url
	FROM Images i
	WHERE i.Id = @Id
END

CREATE PROCEDURE [dbo].[UpdateGallery]
	@Id INT,
	@Name NVARCHAR(50),
	@UserId NVARCHAR(50)
AS
BEGIN
	UPDATE Gallery
	SET Name = @Name
	FROM Gallery g
	WHERE g.Id = @Id
END

CREATE PROCEDURE [dbo].[UpdateProfile]
	@UserId nvarchar(50),
	@FirstName nvarchar(50),
	@LastName nvarchar(max),
	@BirthDate datetime,
	@ImageUrl nvarchar(max)
AS
BEGIN
	EXEC InsertImageIdProfile @UserId, @ImageUrl;

	UPDATE Profiles
	SET FirstName = @FirstName, 
		LastName = @LastName, 
		ImageId = i.maxId,
		BirthDate = @BirthDate
	FROM (select max(Images.Id) as maxId from Images) i
	WHERE Profiles.Email = @UserId
END

CREATE PROCEDURE [dbo].[UpdatePost]
	@Id INT,
	@UserId nvarchar(50),
	@CreateDate datetime,
	@ImageUrl nvarchar(max),
	@Text nvarchar(max)
AS
BEGIN
	EXEC InsertImageIdPost @UserId, @ImageUrl;

	UPDATE Posts
	SET CreateDate = @CreateDate,
		ImageId = i.maxId,
		Text = @Text
	FROM (select max(Images.Id) as maxId from Images) i
	WHERE Posts.Id = @Id
END

CREATE PROCEDURE [dbo].[UpdateComment]
	@Id INT,
	@UserId nvarchar(50),
	@PostId INT,
	@ImageUrl nvarchar(max),
	@Text nvarchar(max)
AS
BEGIN
	EXEC InsertImageIdComment @UserId, @ImageUrl;

	UPDATE Comments
	SET ImageId = i.maxId, 
		Text = @Text 
	FROM (select max(Images.Id) as maxId from Images) i
	WHERE Comments.Id = @Id
END


--If's de insert e update (comments, posts, profiles)

CREATE PROCEDURE [dbo].[InsertImageIdProfile]
	@UserId nvarchar(50),
	@ImageUrl nvarchar(max)
AS
BEGIN
	IF (@ImageUrl IS NOT NULL)
	BEGIN
		IF EXISTS (SELECT g.Name FROM Gallery g WHERE g.Name = 'Profiles')
		BEGIN 
			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Profiles';
		END
		ELSE
		BEGIN
			INSERT INTO Gallery(Name, UserId)
			VALUES ('Profiles', @UserId)

			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Profiles';
		END
	END
END

CREATE PROCEDURE [dbo].[InsertImageIdPost]
	@UserId nvarchar(50),
	@ImageUrl nvarchar(max)
AS
BEGIN
	IF (@ImageUrl IS NOT NULL)
	BEGIN
		IF EXISTS (SELECT g.Name FROM Gallery g WHERE g.Name = 'Posts')
		BEGIN 
			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Posts';
		END
		ELSE
		BEGIN
			INSERT INTO Gallery(Name, UserId)
			VALUES ('Posts', @UserId)

			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Posts';
		END
	END
END

CREATE PROCEDURE [dbo].[InsertImageIdComment]
	@UserId nvarchar(50),
	@ImageUrl nvarchar(max)
AS
BEGIN
	IF(@ImageUrl IS NOT NULL)
	BEGIN
		IF EXISTS (SELECT g.Name FROM Gallery g WHERE g.Name = 'Comments')
		BEGIN 
			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Comments';
		END
		ELSE
		BEGIN
			INSERT INTO Gallery(Name, UserId)
			VALUES ('Comments', @UserId)

			INSERT INTO Images(Url, GalleryId)
			SELECT @ImageUrl, g.Id FROM Gallery g
			WHERE g.Name = 'Comments';
		END
	END
END