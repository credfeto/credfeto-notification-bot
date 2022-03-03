create table twitch.stream_chatter
(
    channel    varchar(100),
    start_date TIMESTAMP,
    chat_user  varchar(100),
    first_message_date TIMESTAMP,
    constraint stream_chatter_pk
        primary key (channel, start_date, chat_user)
);

