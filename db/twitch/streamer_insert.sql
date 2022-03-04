create procedure twitch.streamer_insert(username_ character varying, started_streaming_ timestamp without time zone)
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

alter procedure twitch.streamer_insert(varchar, timestamp) owner to markr;

