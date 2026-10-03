# fire-bot — instruções do repositório

Bot de automação para o jogo Firestone. C# / .NET 6. Sem framework de teste.

## Estrutura do código

- `src/Behaviors/`: comportamento por área (`Guild`, `Map`, `Town`);
- `src/GameModel/`: modelo do jogo; `Features` espelha `Behaviors`;
- `src/Core/`: infraestrutura (`BotTask`, `BotManager`, `Logger`);
- `src/BotActions/`: ações diretas (`AutoSkill`, `AutoUpgrade`, `Hotkey`);
- `src/UI/`: painel e widgets.

## Estado do analyzer

O estado do analyzer não é versionado e fica em `.analyzer/`, na raiz do
repositório. Ele é criado sob demanda pelo agente `analyzer`. Se a pasta não
existir, o agente deve criar a estrutura mínima antes de gravar.

Estrutura esperada:

- `.analyzer/README.md`: taxonomia e ciclo de vida;
- `.analyzer/_global.md`: decisões fora de uma área (`Core`, `UI`,
  `BotActions`);
- `.analyzer/_log.md`: histórico append-only; o agente não deve lê-lo sem
  solicitação explícita;
- `.analyzer/areas/<área>/README.md`: mapa entre código, documentação e
  decisões da área;
- `.analyzer/areas/<área>/<Task>.md`: estado de cada task.

Convenções:

- áreas válidas: `guild`, `map`, `town` e `core`;
- uma task é um arquivo `*Task.cs` em `src/Behaviors/<Área>/`;
- a documentação de uma task é um arquivo, nunca uma pasta;
- quando uma task for compartilhada entre áreas, a área que contém a
  implementação é a fonte de verdade; a outra área apenas aponta para ela;
- `EngineerToolsTask` é o caso compartilhado atual.

Não faça:

- não versione `.analyzer/`;
- não crie uma pasta por task em `.analyzer/areas/`;
- não leia `.analyzer/_log.md` sem pedido explícito;
- não duplique decisões entre áreas; promova decisões transversais para
  `.analyzer/_global.md`.

## Operação do índice RAG

Use o servidor MCP `rag-search` quando a resposta depender do código,
documentação ou decisões deste projeto.

O projeto indexado é `/mnt/z/github/fire-bot` e seu identificador RAG é
`fire-bot`.

O arquivo `/home/demo/ecollm-stack/.env` é a fonte única para o perfil,
conexão com o banco, LiteLLM e credenciais. Não sobrescreva `RAG_PROFILE`,
`RAG_DB_URL`, `LITELLM_BASE_URL`, `QWEN_MAAS_API_KEY` ou outras variáveis
do `.env` na linha de comando.

Não trate uma busca vazia como sucesso. Índice stale, ausência de registro,
erro HTTP 401, erro HTTP 429, timeout, ausência de deployment ou falha de
embedding devem ser reportados como falha.

## Sync do índice

Só execute o sync quando solicitado explicitamente ou quando for necessário
para corrigir um índice ausente ou stale. Antes de executar, verifique o
estado real do repositório e leia as configurações relevantes.

Execute a partir de `/home/demo/ecollm-stack`:

```bash
/home/demo/ecollm-stack/.venv/bin/python -m ingest.cli sync \
  --repo /mnt/z/github/fire-bot
```

Não passe `RAG_PROFILE` ou credenciais na linha de comando.

Depois do sync, valide o resultado no banco:

```bash
docker exec ai-rag-db psql -U rag -d rag -tAc \
"SELECT repo, profile, synced_at, dirty, published_profile
 FROM rag_sync_state
 WHERE repo = 'fire-bot';"
```

Confirme também a quantidade de chunks do repositório `fire-bot` no perfil
ativo. O relatório deve separar comandos executados, evidências observadas,
estado do índice, quantidade de chunks, veredito `PASSA` ou `FALHA` e ação
recomendada.

## Segurança

- Não exponha chaves, tokens ou valores de arquivos `.env`.
- Não edite credenciais ou configurações Docker sem solicitação explícita.
- Não execute comandos destrutivos sem confirmação explícita.
- Preserve o escopo do pedido e valide alterações com as ferramentas
  disponíveis.

## Autoria de commits

- O único contributor do repositório é `danilogmoura`.
- Não adicione trailers `Co-authored-by` ou qualquer outra atribuição a
  Copilot, agentes, modelos ou ferramentas.
- Ao criar commits, preserve a identidade configurada do mantenedor
  `danilogmoura`.
