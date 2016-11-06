# EcsDemo
Lightweight, parallel ECS library demo based on The Witcher.

![alt text](https://github.com/Meeji/EcsDemo/raw/master/ecsdemo1.png "Geralt beats up some drowners Dwarf Fortress style")

## What is it?
This is a simple working demo for a prototype parallel Entity-component-system library. I was interested in the ECS concept and decided to explore it further by writing a simple example. The ECS library allows for simple entity interactions, parallel component updates and exposes a simple fluent interface.

The demo creates several entities based on the Witcher series of games and runs a simulation of them interacting. A more in-depth description of the simulation's rules can be found at the bottom of this readme.

## Overview
The ECS container is created with a number of systems. The systems each manage the associations between entities and components. Components add behaviour and data to an entity, and each entity can have one component of each type. The ECS container can only have one system to manage a certain type of component. This allows one to dynamically add and remove behaviour, data and interactions to entities on-the-fly and without complex and unmanageable class heirarchies.

## Examples
The best way to learn is by example, and with that in mind...

### Creating an ECS container
```cs
Iecs ecs = new Ecs();
```

### Creating an ECS container and adding a system
```cs
var ecs = new Ecs().WithSystem<HasName>();
```

### Creating an entity and giving it a component
```cs
var ecs = new Ecs().WithSystem<HasName>();
var geralt = ecs.NewEntity().WithComponent(new HasName("Geralt")).Entity;
```

alternatively:

```cs
var ecs = new Ecs().WithSystem<HasName>();
var geralt = new Entity();
ecs.ConfigureEntity(geralt).WithComponent(new HasName("Geralt"));
ecs.AddEntity(geralt);
```

### The HasName component

So, what does the ```HasName``` component look like, and how does one retrieve the name of an entity, if it has one?

```cs
public class HasName : Component
{
    public HasName(string name)
    {
        this.Name = name;
    }

    public string Name { get; }

    protected override void Initialise()
    {
    }
}
```

A component extends one of the abstract ```Component```, ```UpdatableComponent``` or ```AsyncUpdatableComponent``` classes. (Note that ```AsyncUpdatableComponent``` extends ```UpdatableComponent``` which itself extends ```Component```). The ```Initialise()``` method is for any initialisation logic that requires access to the ECS container or the associated entity. ```HasName``` does not need to access either so the method is empty.

Retrieving an Entity's name looks like:
```cs
var ecs = new Ecs().WithSystem<HasName>();
var geralt = ecs.NewEntity().WithComponent(new HasName("Geralt")).Entity;
var name = ecs.GetComponent<HasName>(geralt).Name; // name contains "Geralt"
```

### Updatable Components

Components which update once per tick of the simulation extend ```UpdatableComponent``` and are required to implement the ```Update()``` method. A system for an updatable component must also be registered as updatable.

The ```Update()``` method returns an ```Action``` which is run after all components have updated. The reason for this is that if a component wants to change the state of the ECS it can invalidate the other updates in progress. So, a component should do any heavy CPU bound processing in the method body, and act on the result of that calculation in the action. A good example of this is a component which provides AI: it should work out what the entity wants to do this 'tick' in the body of the method, then try to take those actions in the returned ```Action```.

An example updatable ```Component``` which just counts, and prints its count to the console in its final action:
```cs
public class Counts : UpdatableComponent
{
    public int Count { get; private set; }

    public override Action Update()
    {
        this.Count += 1;
        return () => Console.WriteLine(this.Count);
    }

    protected override void Initialise()
    {
    }
}
```

### Asynchronously Updatable Components

An asynchronously updatable ```Component``` should extend ```AsyncUpdatableComponent```. This gives it one extra method to override: ```UpdateAsync()```, which returns a ```Task<Action>```. It works much the same way as the updatable components, except the updates are run in parallel so have to be thread safe. Accessing the ECS's data is thread safe; altering it is not. As with the updatable component, changing the ECS's state must be done in the returned ```Action```.

With a small update the ```Counts``` component now counts in parallel, but reports to the console synchronously.

```cs
 public class CountsAsynchronously : AsyncUpdatableComponent
{
    public int Count { get; private set; }

    public override Action Update()
    {
        this.Count += 1;
        return () => Console.WriteLine(this.Count);
    }
        
    public override Task<Action> UpdateAsync()
    {
        return Task.Run((Func<Action>)this.Update);
    }

    protected override void Initialise()
    {
    }
}
```

### Some more examples

These are heavily annotated examples taken from the demo code.

```cs
private IEcs CreateEcs()
{
    return
        new Ecs().WithSystem<HasName>()
            // This call initialises a basic system for the HasMoney component
            .WithSystem<HasMoney>()
            // This uses a custom system - LocationSystem must implement ISystem<HasLocation>
            .WithCustomSystem<LocationSystem, HasLocation>()
            .WithSystem<Renders>()
            .WithSystem<IsActor>()
            // Registers a system for an UpdatableComponent type
            // If it was registered with WithSystem the components would not get updated per tick
            .WithUpdatableSystem<KilledBy>()
            .WithUpdatableSystem<Talks>()
            // Registers an AsyncUpdatable system
            // If it was registered with WithUpdatableSystem it would update the components synchronously
            .WithAsyncUpdatableSystem<HasAi>();
}
```

```cs
var treeLocations =
    // Call that gets all entities with the IsActor component
    ecs.EntitiesWithComponent<IsActor>() 
        // Then get the subset of entities which have the actor type of 'tree'
        .Where(e => ecs.GetComponent<IsActor>(e).Type == ActorType.Tree)
        // Finally get their locations
        .Select(e => ecs.GetComponent<HasLocation>(e));
```

```cs
// Some lines have been removed from this example.
private Entity CreateGeralt(IEcs ecs)
{
    return
        ecs.NewEntity()
            // Give him some components...
            .WithComponent(new HasName("Geralt"))
            .WithComponent(new Renders('G', ConsoleColor.DarkMagenta))
            .WithComponent(new IsActor(ActorType.Witcher))
            // HasMoney has a parameterless constructor and can be passed just as a type
            .WithComponent<HasMoney>()
            .Entity;
}
```

## Example Simulation
The example simulation tells the epic tale of Geralt the Witcher hunting drowners in a forest. This grant adventure is viewed in glorious 16-colour ASCII art reminiscent of Dwarf Fortress.

The example simulation (Witcher Fortress) creates entities of three types which interact in a number of ways:
* **Geralt**: Geralt is a monster hunter. He presues drowners and avoids trees. He'll occasionally shout taunts or comment on the weather (wind's howling). When he occupies the same space as a drowner he drowner will die and Geralt will gain 5 orens. Sometimes Geralt will have something to say when this happens. Geralt is represented by a purple **G**
* **Drowner**: Drowners will wander aimlessly and gurgle until Geralt is within 5 sqaures of them. Then they'll flee until they're backed up against a tree or the edge of the map. Multiple drowners can occupy the same space. Drowners are represented by yellow **D**s
* **Tree**: Trees get in the way. Trees are represented by green **T**s.

When the simulation starts trees are put in 250 random locations on the map, drowners in 5 random locations and Geralt in a random location. When there are only 2 drowners left, 5 more are placed at a random location.
