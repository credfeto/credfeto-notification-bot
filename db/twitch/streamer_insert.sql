create function twitch.streamer_insert(username_ text, started_streaming_ timestamp with time zone) returns boolean
    language plpgsql
as
$$
begin
    insert into twitch.streamer
    (
        username,
        started_streaming
    )
    values
        (
            username_,
            started_streaming_
        );

    return FOUND;
end
$$;

alter function twitch.streamer_insert(text, timestamp with time zone) owner to markr;

