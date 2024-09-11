INCLUDE GlobalVars.ink
EXTERNAL EnterPortal()

{portal_activated:
    {not portal_notify:
        -> notify_player
      - else:
        -> activated_portal
    }
  - else:
    -> inactive_portal
}
-> END

=== activated_portal ===
# layout: narrator
You look at the now-activated portal, it emits a blue-ish light with a constant low-pitch sound.
The surface of the portal is covered with many tiny white puddles, constantly moving in random direction and bouncing off edges.

# speaker: Dr. Emit
# portrait: dr_emit
# layout: speaker_left
IT'S DONE!
I better hurry up and get the hell out of this madness.
* [Enter the portal.]
    # layout: narrator
    You walk into the portal, and the surroundings begin to distort and fade. Your body feel light, as if it's floating in an empty space.
    After a while, you finally arrive at the main base of TVB, where many familiar faces look at you with <b>mixed</b> expressions.
    ~ EnterPortal()
* [Maybe later.]
    But I still have something to take care of before settling off.
- -> DONE
        

=== inactive_portal ===
# layout: narrator
You approach the portal stand, and its small display shows information about the restoration process, with many complex calculations and tasks constantly being processed.
At the bottom of the screen is a large timer that says: {portal_timer} indicates the remaining time before the process completes.

# speaker: Dr. Emit
# portrait: dr_emit
# layout: speaker_left
It's not ready yet,
After all, restoring the integrity of time sure is not an easy task.
-> DONE


=== notify_player ===
~ portal_notify = true
# layout: narrator
You hear a distinct sound coming from somewhere, perhaps the portal has completed the construction process.
# speaker: Dr. Emit
# portrait: dr_emit
# layout: speaker_left
Oh I recognize that sound! It's the portal's notification sound when it finishes a task!
I should go check that out now.
-> DONE