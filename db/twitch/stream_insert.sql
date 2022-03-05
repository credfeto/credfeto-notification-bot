create function twitch.stream_insert(channel_ character varying, start_date_ timestamp without time zone) returns boolean
    language plpgsql
as
$$
begin
    insert into twitch.stream
    (
        channel,
        start_date
    )
    values
        (
            channel_,
            start_date_
        );

    return found;
end;
$$;

alter function twitch.stream_insert(varchar, timestamp) owner to markr;

