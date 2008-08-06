#print "$_ " foreach @ARGV;
@authout = handle_authors($ARGV[0]);
print "$authout[0]{given}    $authout[0]{family} \n";
print "$authout[1]{given}    $authout[1]{family} \n";
print "$authout[2]{given}    $authout[2]{family} \n";
=pod

=item @authors = Biblio::Citation::Parser::Standard::handle_authors($string)

This (rather large) function handles the author fields of a reference.
It is not all-inclusive yet, but it is usably accurate. It can handle
author lists that are separated by semicolons, commas, and a few other
delimiters, as well as &, and, and 'et al'.

The method takes an author string as a parameter, and returns an array
of extracted information in the format '{family => $family, given =>
$given}'.

=cut 

sub strip_spaces
{	
	my(@bits) = @_;
	foreach(@bits) { s/^[[:space:]]*(.+)[[:space:]]*$/$1/;}
	return @bits;
}
sub handle_authors
{
	my($authstr) = @_;
	my @authsout = ();
	$authstr =~ s/\bet al\b//;
	# Handle semicolon lists
	if ($authstr =~ /;/)
	{
		my @auths = split /[[:space:]]*;[[:space:]]*/, $authstr;
		foreach(@auths)
		{
			my @bits = split /[,[:space:]]+/;
			@bits = strip_spaces(@bits);
			push @authsout, {family => $bits[0], given => $bits[1]};
		}
	}
	elsif ($authstr =~ /^[[:upper:]\.]+[[:space:]]+[[:alnum:]]/)
	{
		my @bits = split /[[:space:]]+/, $authstr;
		@bits = strip_spaces(@bits);
		my $fam = 0;
		my($family, $given);
		foreach(@bits)
		{
			next if ($_ eq "and" || $_ eq "&" || /^[[:space:]]*$/);
			s/,//g;
			if ($fam)
			{
				$family = $_;
				push @authsout, {family => $family, given => $given};
				$fam = 0;
			}
			else
			{
				$given = $_;
				print "Given : $given \n";
				$fam = 1;
			}
		}
	}
	elsif ($authstr =~ /^.+[[:space:]]+[[:upper:]\.]+/)
	{
		# Foo AJ, Bar PJ
		my $fam = 1;
		my $family = "";
		my $given = "";
		my @bits = split /[[:space:]]+/, $authstr;
		@bits = strip_spaces(@bits);
		foreach(@bits)
		{
			s/[,;\.]//g;
			s/\bet al\b//g;
			s/\band\b//;
			s/\b&\b//;
			next if /^[[:space:]]*$/;
			if ($fam == 1)
			{
				$family = $_;
				$fam = 0;
			}
			else
			{
				$given = $_;
				$fam = 1;
				push @authsout, {family => $family, given => $given};
				
			}
		}
	} 
	elsif ($authstr =~ /^.+,[[:space:]]*.+/ || $authstr =~ /.+\band\b.+/)
	{
		my $fam = 1;
		my $family = "";
		my $given = "";
		my @bits = split /[[:space:]]*,|\band\b|&[[:space:]]*/, $authstr;
		@bits = strip_spaces(@bits);
		foreach(@bits)
		{
			next if /^[[:space:]]*$/;
			if ($fam)
			{
				$family = $_;
				$fam = 0;	
			}
			else
			{
				$given = $_;
				push @authsout, {family => $family, given => $given};
				$fam = 1;
			}
		}
	}
	elsif ($authstr =~ /^[[:alpha:][:space:]]+$/)
	{
		$authstr =~ /^([[:alpha:]]+)[[:space:]]*([[:alpha:]]*)$/;
		my $given = "";
		my $family = "";
		if (defined $1 && defined $2)
		{
			$given = $1;
			$family = $2;
		}
		if (!defined $2 || $2 eq "")
		{
			$family = $1;
			$given = "";
		}
		push @authsout, {family => $family, given => $given};
	}
	elsif( $authstr =~ /[[:word:]]+[[:space:]]+[[:word:]]?[[:space:]]*[[:word:]]+/)
	{
		my @bits = split /[[:space:]]+/, $authstr;
		my $rest = $authstr;
		$rest =~ s/$bits[-1]//;
		push @authsout, {family => $bits[-1], given => $rest};
	}
	else
	{
		
	}
	return @authsout;
}


