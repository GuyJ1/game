Welcome to the Game! #speaker:??? #portrait:default #layout:right
-> main

=== main ===
Which faction would you like to see first?
+ [Pirate]
    Ain't nothing worth stealing that I haven't stole yet! Yar har har! #speaker:Allan Vayne #portrait:allen #layout: right
    -> chosen("Pirate")
+ [Merchant]
    If you'd like to make money and a lot of it come and join me! #speaker:Neumann Reluft #portrait:neumann #layout:right
    -> chosen("Merchant")
+ [Monarchy]
    The royals require your service. Should you accept we will wipe your slate clean. #speaker:Rolvo Valeria #portrait:rolvo #layout:right
    -> chosen("Monarchy")
    
=== chosen(faction) ===
Are you sure about {faction}? #speaker:??? #portrait:default #layout:right
+ [Yes]
    -> choice(faction)
+ [No]
    -> main
=== choice(faction) ===
I'm choosing {faction}! #speaker:Fisher Soren #portrait:fisher #layout:left
An excellent choice! Good luck pirate and may your future be fortunate! #speaker:??? #portrait:default #layout:right
->  END
