# Questão 1 - Conta Bancaria

## Comportamentos e Regras

### Cadastro

Deve ser informado:
- o número da conta
- o nome do titular da conta
- Opcional: o valor de depósito inicial que o titular depositou ao abrir a conta.
  
     *O valor de depósito inicial, é opcional, ou seja: se o titular não tiver dinheiro a depositar no momento de abrir sua conta, o depósito inicial não será feito e o saldo inicial da conta será, naturalmente, zero.

Após a conta ser aberta, o número da conta nunca poderá ser alterado. Já o nome do titular pode ser alterado (pois uma pessoa pode mudar de nome quando contrai matrimônio por exemplo).

### Movimentações

O saldo da conta não pode ser alterado livremente. É preciso haver um mecanismo para proteger isso.

O saldo só aumenta por meio de depósitos, e só diminui por meio de saques.

Para cada saque realizado, a instituição cobra uma taxa de $ 3.50.

A conta pode ficar com saldo negativo se o saldo não for suficiente para realizar o saque e/ou pagar a taxa.

## Operações

### Cadastro
Solicita os dados de cadastro da conta, dando opção para que seja ou não informado o valor de depósito inicial. 

### Atualização do cadastro
Permite alterar o nome do titular da conta.

### Depósito
Acrescenta um valor ao saldo da conta

### Saque
Diminui um valor do saldo da conta

### Saldo
Retorna o valor do saldo da conta

## Implementação
### Implementar a classe “ContaBancaria” para que o programa funcione conforme as regras e comportamentos definidos.

### Implementar testes, conforme os exemplos abaixo:

Os testes farão as seguintes operações:
- Cadastro da conta, informando ou não informando um valor inicial
- Realizar um depósito
- Realizar um saque
  
(*) sempre mostrar os dados da conta após cada operação, com a intençaõ de validar o saldo da conta

#### Exemplo 1:

    Entre o número da conta: 5447

    Entre o titular da conta: Milton Gonçalves

    Haverá depósito inicial (s/n)? s

    Entre o valor de depósito inicial: 350.00

    Dados da conta:

    Conta 5447, Titular: Milton Gonçalves, Saldo: $ 350.00

    Entre um valor para depósito: 200

    Dados da conta atualizados:

    Conta 5447, Titular: Milton Gonçalves, Saldo: $ 550.00

    Entre um valor para saque: 199

    Dados da conta atualizados:

    Conta 5447, Titular: Milton Gonçalves, Saldo: $ 347.50

#### Exemplo 2:

    Entre o número da conta: 5139

    Entre o titular da conta: Elza Soares

    Haverá depósito inicial (s/n)? n

    Dados da conta:

    Conta 5139, Titular: Elza Soares, Saldo: $ 0.00

    Entre um valor para depósito: 300.00

    Dados da conta atualizados:

    Conta 5139, Titular: Elza Soares, Saldo: $ 300.00

    Entre um valor para saque: 298.00

    Dados da conta atualizados:

    Conta 5139, Titular: Elza Soares, Saldo: $ -1.50
