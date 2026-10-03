---
name: gate-sync
description: Sincroniza e valida o índice RAG deste projeto
agent: agent
---

Execute o ritual operacional do índice RAG deste projeto.

Não edite arquivos de código, credenciais, `.env` ou configurações Docker.
Não execute comandos destrutivos.
Não adicione trailers `Co-authored-by` ou atribuições a Copilot, agentes,
modelos ou ferramentas; o único contributor é `danilogmoura`.

Leia as configurações reais antes de decidir. Use o `.env` de
`/home/demo/ecollm-stack` como fonte única de perfil, banco, LiteLLM e
credenciais. Não passe variáveis de ambiente ou credenciais na linha de
comando.

O projeto a sincronizar é `/mnt/z/github/fire-bot`, com identificador RAG
`fire-bot`.

Se o sync for necessário, execute:

```bash
cd /home/demo/ecollm-stack
/home/demo/ecollm-stack/.venv/bin/python -m ingest.cli sync \
  --repo /mnt/z/github/fire-bot
```

Depois valide `rag_sync_state` e a quantidade de chunks do repositório
`fire-bot`. Índice stale, ausência de registro, busca vazia, erro HTTP 401,
erro HTTP 429, timeout, ausência de deployment ou falha de embedding são
`FALHA`.

Responda com:

1. comandos executados;
2. evidências observadas;
3. estado e quantidade do índice;
4. veredito `PASSA` ou `FALHA`;
5. ação recomendada.
