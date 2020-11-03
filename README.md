# cac-cli

This name comes from: **C**onfiguration **a**s **C**ode.

Configuration as code (CAC) is managing configuration resources in your source repository. You treat your application config resources as versioned artifacts. By managing your application environment in tandem with your application code, you gain the same benefits you get with your code. CAC is a set of processes and practices that will save you time, increase your flexibility, and improve your system uptime.

- Simplify Configuration Data
- Take Advantage of Version Control
- IAC and CAC both together

**cac-cli** will help you appling CAC.

You can read more about it in the [wiki page](https://github.com/fernandoescolar/cac-cli/wiki).

You can check latests releases in the [releases page](https://github.com/fernandoescolar/cac-cli/releases).

## Hello world

Install latest version using `dotnet` client:

> dotnet tool install --global cac-cli --version 0.0.1-beta1

Create a new file called `file.yml` with the content bellow:

```yaml
parameters:
  environment:

packages:
- name: cac.sample
  version: 0.0.1-beta1

providers:
- sample: sample
  var1: ${{parameters.environment}}1
  var2: ${{parameters.environment}}2
  var3: ${{parameters.environment}}3

variables:
  list:
  - name: ${{ providers.sample.var1 }}
  - name: ${{ providers.sample.var2 }}
  - name: ${{ providers.sample.var3 }}

activities:
- for_each: ${{ variables.list }}
  as: item
  activities:
  - sample: ${{ item.name }}
```

Execute `plan` CAC:

> cac plan file.yml -p environment=dev

You should find the output bellow in your terminal:

```bash
packages
  Downloading: cac.sample v0.0.1-beta1
plan
  Name to write: dev1
  Name to write: dev2
  Name to write: dev3
```

## License

The source code we develop at **cac-cli** is default being licensed as CC-BY-SA-4.0. You can read more about [here](LICENSE.md).