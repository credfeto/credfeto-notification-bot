CREATE FUNCTION twitch.stream_follower_insert (
    channel_ TEXT,
    follower_ TEXT
    )
RETURNS TABLE (
    channel VARCHAR,
    follower VARCHAR,
    follow_count INT,
    freshly_reached boolean
    ) LANGUAGE plpgsql
AS
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
        ON conflict
            ON CONSTRAINT stream_follower_pk do

    UPDATE
    SET follow_count = stream_follower.follow_count + 1,
        last_followed = now();

    RETURN query(SELECT f.channel, f.follower, f.follow_count, f.follow_count = 1 AS freshly_reached FROM twitch.stream_follower f WHERE f.channel = channel_
            AND f.follower = follower_);
END;$$;


ALTER FUNCTION twitch.stream_follower_insert (
    TEXT,
    TEXT
    ) OWNER TO markr;


GRANT EXECUTE ON FUNCTION twitch.stream_follower_insert (
    TEXT,
    TEXT
    ) TO markr;
