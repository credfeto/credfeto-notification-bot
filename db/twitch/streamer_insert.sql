create procedure twitch.streamer_insert(username_ text, started_streaming_ timestamp with time zone)
    language sql
as
$$
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
$$;

alter procedure twitch.streamer_insert(text, timestamp with time zone) owner to markr;

