files = Dir["*.cs"]
lines = 0
puts files
for i in 0 ... files.length()
    lines += File.foreach(files[i]).inject(0) {|c, line| c+1}
end

puts "Total Lines: " + lines.to_s