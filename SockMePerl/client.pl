#!/usr/bin/env perl
use v5.30;
use warnings;

use IO::Socket qw(AF_INET AF_UNIX SOCK_STREAM SHUT_WR);

my $client = IO::Socket->new(
    Domain => AF_INET,
    Type => SOCK_STREAM,
    proto => 'tcp',
    PeerPort => 6969,
    PeerHost => '0.0.0.0',
) || die "Can't open socket: $IO::Socket::errstr";

say "Sending Hello World!";
my $size = $client->send("Hello World!");
say "Sent data of length: $size";

$client->shutdown(SHUT_WR);

my $buffer;
$client->recv($buffer, 1024);
say "Got back $buffer";

$client->close();
