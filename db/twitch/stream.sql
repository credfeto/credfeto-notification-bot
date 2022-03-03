create table twitch.stream
(
    channel    varchar(100) not null,
    start_date date,
    constraint stream_pk
        primary key (channel, start_date)
);

