Select u.username
from (select v.username
      from twitch.viewer v
      union
      select s.username
      from twitch.streamer s) u;

select f.follower            as username,
       count(f.channel)      as channels,
       max(f.follow_count)   as max_follow_count,
       min(f.follow_count)   as min_follow_count,
       avg(f.follow_count)   as avg_follow_count,
       min(f.first_followed) as first_follow_date
from twitch.stream_follower f
group by f.follower;

select c.chat_user,
       count(distinct c.channel) as chat_channels,
       count(*)                  as chat_streams,
       min(c.start_date)         as first_stream,
       max(c.start_date)         as last_stream
from twitch.stream_chatter c
group by c.chat_user;
