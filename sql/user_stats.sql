SELECT u.username
FROM (
    SELECT v.username
    FROM twitch.viewer v
    
    UNION
    
    SELECT s.username
    FROM twitch.streamer s
    ) u;

SELECT f.follower AS username,
    count(f.channel) AS channels,
    max(f.follow_count) AS max_follow_count,
    min(f.follow_count) AS min_follow_count,
    avg(f.follow_count) AS avg_follow_count,
    min(f.first_followed) AS first_follow_date
FROM twitch.stream_follower f
GROUP BY f.follower;

SELECT c.chat_user,
    count(DISTINCT c.channel) AS chat_channels,
    count(*) AS chat_streams,
    min(c.start_date) AS first_stream,
    max(c.start_date) AS last_stream
FROM twitch.stream_chatter c
GROUP BY c.chat_user;
