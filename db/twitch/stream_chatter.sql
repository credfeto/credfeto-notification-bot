create table twitch.stream_chatter
(
    channel    varchar(100),
    start_date date,
    chat_user  varchar(100),
    first_chat date,
    constraint stream_chatter_pk
        primary key (channel, start_date, chat_user)
);

