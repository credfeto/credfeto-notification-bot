CREATE FUNCTION twitch.stream_settings_set (
    channel_ VARCHAR,
    start_date_ TIMESTAMP WITH TIME zone,
    thanks_ boolean,
    announce_milestones_ boolean,
    chat_welcomes_ boolean,
    raid_welcomes_ boolean,
    shout_outs_ boolean
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    INSERT INTO twitch.stream_settings (
        channel,
        start_date,
        thanks,
        announce_milestones,
        chat_welcomes,
        raid_welcomes,
        shout_outs
        )
    VALUES (
        channel_,
        start_date_,
        thanks_,
        announce_milestones_,
        chat_welcomes_,
        raid_welcomes_,
        shout_outs_
        )
        ON conflict(channel, start_date) do

    UPDATE
    SET thanks = excluded.thanks,
        announce_milestones = excluded.announce_milestones,
        chat_welcomes = excluded.chat_welcomes,
        raid_welcomes = excluded.raid_welcomes,
        shout_outs = excluded.shout_outs;

    RETURN found;
END;$$;

ALTER FUNCTION twitch.stream_settings_set (
    VARCHAR,
    TIMESTAMP WITH TIME zone,
    boolean,
    boolean,
    boolean,
    boolean,
    boolean
    ) OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.stream_settings_set(VARCHAR, TIMESTAMP WITH TIME zone, boolean, boolean, boolean, boolean, boolean)
    TO notificationbot;
