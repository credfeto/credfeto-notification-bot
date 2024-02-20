CREATE FUNCTION twitch.stream_settings_get (
    channel_ VARCHAR,
    start_date_ TIMESTAMP WITH TIME zone
    )
RETURNS TABLE (
    channel VARCHAR,
    start_date TIMESTAMP without TIME zone,
    thanks boolean,
    announce_milestones boolean,
    chat_welcomes boolean,
    raid_welcomes boolean,
    shout_outs boolean
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    RETURN QUERY(SELECT s.channel, s.start_date, s.thanks, s.announce_milestones, s.chat_welcomes, s.raid_welcomes, s.shout_outs FROM twitch.stream_settings s WHERE s.channel = channel_
            AND s.start_date = start_date_);
END;$$;

ALTER FUNCTION twitch.stream_settings_get (
    VARCHAR,
    TIMESTAMP WITH TIME zone
    ) OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.stream_settings_get(VARCHAR, TIMESTAMP WITH TIME zone)
    TO notificationbot;
