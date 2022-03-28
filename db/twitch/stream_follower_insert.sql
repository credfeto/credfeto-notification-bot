create function twitch.stream_follower_insert(channel_ character varying, follower_ character varying)
    returns TABLE(channel character varying, follower character varying, follow_count integer, freshly_reached boolean)
    language plpgsql
as
$$
BEGIN
    INSERT INTO twitch.stream_follower (
        channel,
        follower,
        follow_count,
        first_followed,
        last_followed
    )
    VALUES (
               channel_,
               follower_,
               1,
               now(),
               now()
           )
    ON conflict do update set
        follow_count = stream_follower.follow_count + 1,
        last_followed = now();

    RETURN query (
        SELECT f.channel,
               f.follower,
               f.follow_count,
               f.follow_count == 1 as freshly_reached
        FROM twitch.stream_follower f
        WHERE f.channel = channel_
          AND f.follower = follower_
    );        
END;
$$;

alter function twitch.stream_follower_insert(varchar, varchar) owner to markr;

