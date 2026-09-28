# The Hycrest Insurrection (adventure, world 1149)

Work in progress. This is the first playable part of the Exile level 15 adventure: the intro on the drop ship and the
first mission vote in the Abandoned Barn. The missions themselves aren't implemented yet.

## What's in it

- **Arrival:** players enter the adventure standing on the Dominion drop ship. After a short black screen the ship flies
  to its spot above the Abandoned Orchards and turns into place, with the players on board. The players get the green
  "synchronisation" glow and the Caretaker's two messages, then Vice-Marshal Dawson steps out of the door.
- **Briefing:** talking to Dawson (objective 2113) starts his 20 s briefing (2155). If nobody talks to him within 60 s
  the briefing starts by itself.
- **The jump:** the exit door opens, players walk down the ramp and jump; the slow-burn jetpack (Rocket Fall) keeps them
  gliding until they land. The ship leaves once everyone is off (players still on board after 30 s are dropped at the
  ramp).
- **Barn:** "Meet with Vesna Taranoft" (189) waits for the whole party. Vesna, Ayita and Lysion brief the players and
  pitch their missions, then the mission vote (45) starts. The chosen mission (420, 421 or 422) is started, but has no
  content yet.
- **Exit portal:** "Leave Simulation", the green portal in the Abandoned Orchards where players land, takes players back
  to where they entered the adventure from and ends the run (the next entry is a fresh instance).

## Requirements

- The world database spawns: `Instance/Adventure/The Hycrest Insurrection.sql` from NexusForever.WorldDatabase. Import
  it into your world database (it removes and re-adds everything for world 1149, so it can be imported again), e.g.:
  ```
  mysql -u <user> -p nexus_forever_world < "Instance/Adventure/The Hycrest Insurrection.sql"
  ```
  Spawns are read when an instance is created, so no restart is needed; the map entrance is read at startup.
- The engine changes this builds on: #577 (objective timers, TimedWin), #578, #579, #580 (players on platforms), #581
  (return location), #582 (instance portals), #583 (party size for "Meet with Vesna") and `!map fresh`
  (`IMapLockManager.RemoveSoloLock`).
- The night sky (spell 27236) needs SpellForceRemove not to remove the spell that runs it; without that the adventure
  plays in daylight.

## Getting in

The group finder entrance (`map_entrance` 1149, location 13039) is in the SQL, but queueing hasn't been tested. Use a GM
teleport instead, which needs a GM or Administrator account.

**Give your account the Administrator role**, either:

- in the world server console (or the web console), create a new account with the role:
  ```
  account create <email> <password> Administrator
  ```
- or for an existing account, in the auth database (role 3 is Administrator; find the account id in `account`):
  ```sql
  INSERT INTO nexus_forever_auth.account_role (id, roleId) VALUES (<account id>, 3);
  ```
  Log out and back in afterwards.

**Enter the adventure** from anywhere outside it (level 15 recommended):

1. Clear your target (Esc): commands act on your current target.
2. Type `!teleport location 13039`.
3. The first load of world 1149 takes a while. You arrive on the drop ship.

To play it again, either:

- type `!map fresh` inside the adventure: you are moved into a new instance of it right away;
- or leave through the "Leave Simulation" portal and teleport in again.

Otherwise the instance stays until it unloads (`GridUnloadTimer`: 3600 s in `WorldServer.example.json`) and teleporting
in again puts you back into it.

## Known gaps

- No NPC voice lines yet.
- Retail's arrival cinematic isn't reproduced; the black screen only covers the players settling on the ship.
- Only the players glow green on arrival; in retail the whole view turned green.
- Tested solo; group play (votes, "Waiting for N more") is untested.
