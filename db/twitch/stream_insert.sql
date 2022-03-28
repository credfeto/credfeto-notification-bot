create function twitch.stream_insert(channel_ character varying, start_date_ timestamp with time zone) returns boolean
    language plpgsql
as
$$
BEGIN
    INSERT INTO twitch.stream (
        channel,
        start_date
    )
    VALUES (
               channel_,
               start_date_
           )
    ON conflict do nothing;

    RETURN found;
END;
$$;

alter function twitch.stream_insert(varchar, timestamp with time zone) owner to markr;

grant execute on function twitch.stream_insert(varchar, timestamp with time zone) to notificationbot;

