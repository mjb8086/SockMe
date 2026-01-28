#!/usr/bin/env perl
use v5.30;
use warnings;

use IO::Socket::IP;

my $client = IO::Socket::IP->new(
   PeerHost => "localhost",
   PeerPort => 11_000,
   proto => 'tcp',
   Type  => SOCK_STREAM,
) or die "Cannot construct socket - $IO::Socket::errstr";

say "Sending Hello World!";
my $size = $client->send("Hello Perld!<|EOM|>");
say "Sent data of length: $size";

$client->shutdown(SHUT_WR);

my $buffer;
$client->recv($buffer, 1024);
say "Got back $buffer";

$client->close();
