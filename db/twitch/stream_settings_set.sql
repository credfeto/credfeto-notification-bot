CREATE FUNCTION twitch.stream_settings_set(
    channel_ VARCHAR,
    start_date_ TIMESTAMP WITH TIME zone,
    thanks_ BIT,
    announce_milestones_ BIT,
    chat_welcomes_ BIT,
    raid_welcomes_ BIT,
    shout_outs_ BIT
)
    RETURNS boolean
    LANGUAGE plpgsql
AS
$$

BEGIN
    INSERT INTO twitch.stream_settings (channel,
                                        start_date,
                                        thanks,
                                        announce_milestones,
                                        chat_welcomes,
                                        raid_welcomes,
                                        shout_outs)
    VALUES (channel_,
            start_date_,
            thanks,
            announce_milestones,
            chat_welcomes,
            raid_welcomes,
            shout_outs)
    ON conflict do update
        set thanks              = thanks_,
            announce_milestones = announce_milestones_,
            chat_welcomes       = chat_welcomes_,
            raid_welcomes       = raid_welcomes_,
            shout_outs          = shout_outs_
    where channel = channel_
      and start_date = start_date;

    RETURN found;
END;
$$;

ALTER FUNCTION twitch.stream_settings_set (
    VARCHAR,
    TIMESTAMP WITH TIME zone,
    BIT,
    BIT,
    BIT,
    BIT,
    BIT
    ) OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.stream_settings_set(VARCHAR, TIMESTAMP WITH TIME zone,
    BIT,
    BIT,
    BIT,
    BIT,
    BIT)
    TO notificationbot;
