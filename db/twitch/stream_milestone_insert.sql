CREATE FUNCTION twitch.stream_milestone_insert (
    channel_ VARCHAR,
    followers_ INT
    )
RETURNS TABLE (
    channel VARCHAR,
    followers INT,
    freshly_reached boolean
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    INSERT INTO twitch.streamer_milestone (
        channel,
        followers,
        date_created
        )
    VALUES (
        channel_,
        followers_,
        now()
        )
        ON conflict do nothing;

    RETURN query(SELECT s.channel AS channel, max(s.followers) AS followers, found
            AND max(s.followers) = followers_ AS freshly_reached FROM twitch.streamer_milestone AS s WHERE s.channel = channel_ GROUP BY s.channel);
END;$$;

ALTER FUNCTION twitch.stream_milestone_insert (
    VARCHAR,
    INT
    ) OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.stream_milestone_insert(VARCHAR, INT)
    TO notificationbot;
