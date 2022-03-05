create function twitch.stream_insert(channel_ character varying, start_date_ timestamp with time zone) returns boolean
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
        )
    on conflict do nothing;

    return found;
end;
$$;

alter function twitch.stream_insert(varchar, timestamp with time zone) owner to markr;

